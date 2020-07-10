namespace MoqExtensions.HttpResponseMessage
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Moq.Protected;

    [ExcludeFromCodeCoverage]
    public class MockRequestConfiguration
    {
        public static MockRequestConfiguration<TReq, TResp> New<TReq, TResp>() where TReq : class where TResp : class
        {
            return new MockRequestConfiguration<TReq, TResp>();
        }

        public static MockRequestConfiguration<TReq, object> New<TReq>() where TReq : class
        {
            return new MockRequestConfiguration<TReq, object>();
        }
    }

    public class MockRequestConfiguration<TReq, TResp> where TReq : class where TResp : class
    {
        public HttpMethod RequestMethod { get; private set; }
        public MockRequestConfiguration<TReq, TResp> WithRequestMethod(HttpMethod requestMethod)
        {
            RequestMethod = requestMethod;
            return this;
        }

        public Uri RequestUri { get; private set; }
        public MockRequestConfiguration<TReq, TResp> WithRequestAddress(string requestAddress)
        {
            RequestUri = new Uri(requestAddress);
            return this;
        }

        public Action<HttpRequestMessage> OriginalRequestAction { get; private set; }
        public MockRequestConfiguration<TReq, TResp> RetrieveRequestMessageAt(Action<HttpRequestMessage> originalRequestAction)
        {
            OriginalRequestAction = originalRequestAction;
            return this;
        }

        public Action<TReq> OriginalRequestContentAction { get; private set; }
        public MockRequestConfiguration<TReq, TResp> RetrieveRequestMessageContentAt(Action<TReq> originalRequestContentAction)
        {
            OriginalRequestContentAction = originalRequestContentAction;
            return this;
        }

        public HttpStatusCode ResponseStatusCode { get; private set; } = HttpStatusCode.OK;
        public MockRequestConfiguration<TReq, TResp> WithResponseStatusCode(HttpStatusCode responseStatusCode)
        {
            ResponseStatusCode = responseStatusCode;
            return this;
        }

        public TResp ResponseContent { get; private set; }
        public MockRequestConfiguration<TReq, TResp> WithResponseContent(TResp responseContent)
        {
            ResponseContent = responseContent;
            return this;
        }

        public void BuildAt(Mock<HttpMessageHandler> mock)
        {
            Expression requestMessageExpression;
            
            #region Defining the requestMessageExpression

            if (RequestMethod == null && RequestUri == null)
                requestMessageExpression = ItExpr.IsAny<HttpRequestMessage>();
            else if (RequestMethod != null)
                requestMessageExpression = ItExpr.Is<HttpRequestMessage>(x => x.Method == RequestMethod);
            else if (RequestUri != null)
                requestMessageExpression = ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == RequestUri);
            else
                requestMessageExpression = ItExpr.Is<HttpRequestMessage>(x => x.Method == RequestMethod && x.RequestUri == RequestUri);
            
            #endregion

            var response = new HttpResponseMessage(ResponseStatusCode);
            if (ResponseContent != null)
            {
                if (ResponseContent is string content)
                    response.Content = new StringContent(content);
                else
                    response.Content = new StringContent(JsonSerializer.Serialize(ResponseContent));
            }

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", requestMessageExpression, ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) =>
                {
                    OriginalRequestAction?.Invoke(message);
                    OriginalRequestContentAction?.Invoke(JsonSerializer.Deserialize<TReq>(message.Content.ReadAsStringAsync().Result));
                })
                .ReturnsAsync(response)
                .Verifiable();
        }
    }
}
