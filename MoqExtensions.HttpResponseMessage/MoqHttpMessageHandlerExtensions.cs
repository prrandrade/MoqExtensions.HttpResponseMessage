namespace MoqExtensions.HttpResponseMessage
{
    using System;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Moq.Protected;

    public static class MoqHttpMessageHandlerExtensions
    {
        #region Mock<HttpMessageHandler> without customized HttpRequestMessage

        /// <summary>
        /// Setup a HTTP response message with no content and status code 200
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with no content and customized status code
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="responseStatusCode">The status code that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, HttpStatusCode responseStatusCode, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(responseStatusCode))
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with customized content and status code 200
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="responseContent">The content that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = responseContent
                })
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with customized content and customized status code
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="responseStatusCode">The status code that the response will return</param>
        /// <param name="responseContent">The content that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, HttpStatusCode responseStatusCode, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(responseStatusCode)
                {
                    Content = responseContent
                })
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        #endregion

        #region Mock<HttpMessageHandler> with customized HttpRequestMessage

        /// <summary>
        /// Setup a HTTP response message with no content and status code 200
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="httpRequestMessage">Customized HttpRequestMessage that will be handled with this mock setup</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, HttpRequestMessage httpRequestMessage, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", httpRequestMessage, ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with no content and customized status code
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="httpRequestMessage">Customized HttpRequestMessage that will be handled with this mock setup</param>
        /// <param name="responseStatusCode">The status code that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, HttpRequestMessage httpRequestMessage, HttpStatusCode responseStatusCode, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", httpRequestMessage, ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(responseStatusCode))
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with customized content and status code 200
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="httpRequestMessage">Customized HttpRequestMessage that will be handled with this mock setup</param>
        /// <param name="responseContent">The content that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, HttpRequestMessage httpRequestMessage, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", httpRequestMessage, ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = responseContent
                })
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with customized content and customized status code
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="httpRequestMessage">Customized HttpRequestMessage that will be handled with this mock setup</param>
        /// <param name="responseStatusCode">The status code that the response will return</param>
        /// <param name="responseContent">The content that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, HttpRequestMessage httpRequestMessage, HttpStatusCode responseStatusCode, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", httpRequestMessage, ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(responseStatusCode)
                {
                    Content = responseContent
                })
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        #endregion

        #region Mock<HttpMessageHandler> with customized Expression<Func<HttpRequestMessage, bool>>

        /// <summary>
        /// Setup a HTTP response message with no content and status code 200
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="httpRequestMessageExpression">Expression that matches the desired HttpRequestMessage that will be handled</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> httpRequestMessageExpression, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is(httpRequestMessageExpression), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with no content and customized status code
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="httpRequestMessageExpression">Expression that matches the desired HttpRequestMessage that will be handled</param>
        /// <param name="responseStatusCode">The status code that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> httpRequestMessageExpression, HttpStatusCode responseStatusCode, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is(httpRequestMessageExpression), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(responseStatusCode))
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with customized content and status code 200
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="httpRequestMessageExpression">Expression that matches the desired HttpRequestMessage that will be handled</param>
        /// <param name="responseContent">The content that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> httpRequestMessageExpression, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is(httpRequestMessageExpression), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = responseContent
                })
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        /// <summary>
        /// Setup a HTTP response message with customized content and customized status code
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> used for mocking</param>
        /// <param name="httpRequestMessageExpression">Expression that matches the desired HttpRequestMessage that will be handled</param>
        /// <param name="responseStatusCode">The status code that the response will return</param>
        /// <param name="responseContent">The content that the response will return</param>
        /// <param name="action">Optionally this action will receive the original HttpRequestMessage</param>
        /// <param name="verifiableDispose">True if the dispose method can be verified. Recommended if the HttpClient is called is a disposable manner.</param>
        public static void SetupHttpResponseMessage(this Mock<HttpMessageHandler> mock, Expression<Func<HttpRequestMessage, bool>> httpRequestMessageExpression, HttpStatusCode responseStatusCode, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)
        {
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is(httpRequestMessageExpression), ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((message, token) => action?.Invoke(message))
                .ReturnsAsync(new HttpResponseMessage(responseStatusCode)
                {
                    Content = responseContent
                })
                .Verifiable();

            if (verifiableDispose)
                mock.Protected()
                    .Setup("Dispose", ItExpr.IsAny<bool>())
                    .Verifiable();
        }

        #endregion

        /// <summary>
        /// Creates a HttpClient that uses the passed Mock<![CDATA[<HttpMessageHandler>]]>
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> that the new HttpClient will use</param>
        /// <param name="baseAddress">The HttpClient base address</param>
        /// <returns>A new HttpClient with a customized HttpMessageHandler</returns>
        public static HttpClient CreateHttpClientMock(this Mock<HttpMessageHandler> mock, string baseAddress)
        {
            return new HttpClient(mock.Object)
            {
                BaseAddress = new Uri(baseAddress)
            };
        }
    }
}
