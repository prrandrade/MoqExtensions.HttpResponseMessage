namespace MoqExtensions.HttpResponseMessage.UnitTest
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Moq.Protected;
    using Xunit;
    using static MoqHttpMessageHandlerExtensions;

    [ExcludeFromCodeCoverage]
    public class MoqHttpMessageHandlerExtensionsTests
    {
        #region Mock<HttpMessageHandler> without customized HttpRequestMessage

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        public async Task SetupHttpResponseMessage(bool withAction, bool verifiableDispose)
        {
            // arrange
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(action: request => actionIsCalled = true, verifiableDispose: verifiableDispose);
            else
                mock.SetupHttpResponseMessage(verifiableDispose: verifiableDispose);

            if (verifiableDispose)
                using (var httpClient = new HttpClient(mock.Object))
                {
                    responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/"));
                }
            else
            {
                var httpClient = new HttpClient(mock.Object);
                responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/"));
            }

            // assert
            Assert.Null(responseMessage.Content);
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            if (withAction) Assert.True(actionIsCalled);

            if (verifiableDispose)
                mock.Verify();
            else
            {
                mock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == "http://www.site.com.br/"), ItExpr.IsAny<CancellationToken>());
                mock.Protected().Verify("Dispose", Times.Never(), ItExpr.IsAny<bool>());
            }
        }

        [Theory]
        [InlineData(false, HttpStatusCode.BadGateway, true)]
        [InlineData(true, HttpStatusCode.Created, true)]
        [InlineData(false, HttpStatusCode.BadGateway, false)]
        [InlineData(true, HttpStatusCode.Created, false)]
        public async Task SetupHttpResponseMessage_WithHttpStatusCode(bool withAction, HttpStatusCode responseStatusCode, bool verifiableDispose)
        {
            // arrange
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(responseStatusCode, action: request => actionIsCalled = true, verifiableDispose);
            else
                mock.SetupHttpResponseMessage(responseStatusCode, verifiableDispose: verifiableDispose);

            if (verifiableDispose)
                using (var httpClient = new HttpClient(mock.Object))
                {
                    responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/"));
                }
            else
            {
                var httpClient = new HttpClient(mock.Object);
                responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/"));
            }

            // assert
            Assert.Null(responseMessage.Content);
            Assert.Equal(responseStatusCode, responseMessage.StatusCode);
            if (withAction) Assert.True(actionIsCalled);
            if (verifiableDispose)
                mock.Verify();
            else
            {
                mock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == "http://www.site.com.br/"), ItExpr.IsAny<CancellationToken>());
                mock.Protected().Verify("Dispose", Times.Never(), ItExpr.IsAny<bool>());
            }
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        public async Task SetupHttpResponseMessage_WithContent(bool withAction, bool verifiableDispose)
        {
            // arrange
            var responseContent = new StringContent("");
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(responseContent: responseContent, action: request => actionIsCalled = true, verifiableDispose: verifiableDispose);
            else
                mock.SetupHttpResponseMessage(responseContent: responseContent, verifiableDispose: verifiableDispose);

            if (verifiableDispose)
                using (var httpClient = new HttpClient(mock.Object))
                {
                    responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/"));
                }
            else
            {
                var httpClient = new HttpClient(mock.Object);
                responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/"));
            }

            // assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            Assert.Equal(responseContent, responseMessage.Content);
            if (withAction) Assert.True(actionIsCalled);
            if (verifiableDispose)
                mock.Verify();
            else
            {
                mock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == "http://www.site.com.br/"), ItExpr.IsAny<CancellationToken>());
                mock.Protected().Verify("Dispose", Times.Never(), ItExpr.IsAny<bool>());
            }
        }

        [Theory]
        [InlineData(false, HttpStatusCode.BadGateway, true)]
        [InlineData(true, HttpStatusCode.Created, true)]
        [InlineData(false, HttpStatusCode.BadGateway, false)]
        [InlineData(true, HttpStatusCode.Created, false)]
        public async Task SetupHttpResponseMessage_WithHttpStatusCodeAndContent(bool withAction, HttpStatusCode responseStatusCode, bool verifiableDispose)
        {
            // arrange
            var responseContent = new StringContent("");
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(responseStatusCode, responseContent: responseContent, action: request => actionIsCalled = true, verifiableDispose: verifiableDispose);
            else
                mock.SetupHttpResponseMessage(responseStatusCode, responseContent: responseContent, verifiableDispose: verifiableDispose);

            if (verifiableDispose)
                using (var httpClient = new HttpClient(mock.Object))
                {
                    responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/"));
                }
            else
            {
                var httpClient = new HttpClient(mock.Object);
                responseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/"));
            }

            // assert
            Assert.Equal(responseStatusCode, responseMessage.StatusCode);
            Assert.Equal(responseContent, responseMessage.Content);
            if (withAction) Assert.True(actionIsCalled);
            if (verifiableDispose)
                mock.Verify();
            else
            {
                mock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(x => x.Method == HttpMethod.Get && x.RequestUri.AbsoluteUri == "http://www.site.com.br/"), ItExpr.IsAny<CancellationToken>());
                mock.Protected().Verify("Dispose", Times.Never(), ItExpr.IsAny<bool>());
            }
        }

        #endregion

        #region Mock<HttpMessageHandler> with customized HttpRequestMessage

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetupHttpResponseMessage_WithRequest(bool withAction)
        {
            // arrange
            var mockedRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/");
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(mockedRequestMessage, action: request =>
                {
                    actionIsCalled = true;
                    requestMessage = request;
                });
            else
                mock.SetupHttpResponseMessage(mockedRequestMessage);

            using (var httpClient = new HttpClient(mock.Object))
            {
                responseMessage = await httpClient.SendAsync(mockedRequestMessage);
            }

            // assert
            Assert.Null(responseMessage.Content);
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            if (withAction) Assert.True(actionIsCalled);
            if (withAction) Assert.Equal(mockedRequestMessage, requestMessage);
            mock.Verify();
        }

        [Theory]
        [InlineData(false, HttpStatusCode.BadGateway)]
        [InlineData(true, HttpStatusCode.Created)]
        public async Task SetupHttpResponseMessage_WithRequest_WithHttpStatusCode(bool withAction, HttpStatusCode responseStatusCode)
        {
            // arrange
            var mockedRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/");
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(mockedRequestMessage, responseStatusCode, action: request =>
                {
                    actionIsCalled = true;
                    requestMessage = request;
                });
            else
                mock.SetupHttpResponseMessage(mockedRequestMessage, responseStatusCode);

            using (var httpClient = new HttpClient(mock.Object))
            {
                responseMessage = await httpClient.SendAsync(mockedRequestMessage);
            }

            // assert
            Assert.Null(responseMessage.Content);
            Assert.Equal(responseStatusCode, responseMessage.StatusCode);
            if (withAction) Assert.True(actionIsCalled);
            if (withAction) Assert.Equal(mockedRequestMessage, requestMessage);
            mock.Verify();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetupHttpResponseMessage_WithRequest_WithContent(bool withAction)
        {
            // arrange
            var mockedRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/");
            var responseContent = new StringContent("");
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(mockedRequestMessage, responseContent: responseContent, action: request =>
                {
                    actionIsCalled = true;
                    requestMessage = request;
                });
            else
                mock.SetupHttpResponseMessage(mockedRequestMessage, responseContent: responseContent);

            using (var httpClient = new HttpClient(mock.Object))
            {
                responseMessage = await httpClient.SendAsync(mockedRequestMessage);
            }

            // assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            Assert.Equal(responseContent, responseMessage.Content);
            if (withAction) Assert.True(actionIsCalled);
            if (withAction) Assert.Equal(mockedRequestMessage, requestMessage);
            mock.Verify();
        }

        [Theory]
        [InlineData(false, HttpStatusCode.BadGateway)]
        [InlineData(true, HttpStatusCode.Created)]
        public async Task SetupHttpResponseMessage_WithRequest_WithHttpStatusCodeAndContent(bool withAction, HttpStatusCode responseStatusCode)
        {
            // arrange
            var mockedRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.site.com.br/");
            var responseContent = new StringContent("");
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(mockedRequestMessage, responseStatusCode, responseContent: responseContent, action: request =>
                {
                    actionIsCalled = true;
                    requestMessage = request;
                });
            else
                mock.SetupHttpResponseMessage(mockedRequestMessage, responseStatusCode, responseContent: responseContent);

            using (var httpClient = new HttpClient(mock.Object))
            {
                responseMessage = await httpClient.SendAsync(mockedRequestMessage);
            }

            // assert
            Assert.Equal(responseStatusCode, responseMessage.StatusCode);
            Assert.Equal(responseContent, responseMessage.Content);
            if (withAction) Assert.True(actionIsCalled);
            if (withAction) Assert.Equal(mockedRequestMessage, requestMessage);
            mock.Verify();
        }

        #endregion

        #region Mock<HttpMessageHandler> with customized Expression<Func<HttpRequestMessage, bool>>

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetupHttpResponseMessage_WithCustomizedRequest(bool withAction)
        {
            // arrange
            const string requestUri = "http://www.site.com.br/";
            var mockedRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(request => request.RequestUri == new Uri(requestUri), action: request =>
                {
                    actionIsCalled = true;
                    requestMessage = request;
                });
            else
                mock.SetupHttpResponseMessage(request => request.RequestUri == new Uri(requestUri));

            using (var httpClient = new HttpClient(mock.Object))
            {
                responseMessage = await httpClient.SendAsync(mockedRequestMessage);
            }

            // assert
            Assert.Null(responseMessage.Content);
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            if (withAction) Assert.True(actionIsCalled);
            if (withAction) Assert.Equal(requestUri, requestMessage.RequestUri.AbsoluteUri);
            mock.Verify();
        }

        [Theory]
        [InlineData(false, HttpStatusCode.BadGateway)]
        [InlineData(true, HttpStatusCode.Created)]
        public async Task SetupHttpResponseMessage_WithCustomizedRequest_WithHttpStatusCode(bool withAction, HttpStatusCode responseStatusCode)
        {
            // arrange
            const string requestUri = "http://www.site.com.br/";
            var mockedRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(request => request.RequestUri == new Uri(requestUri), responseStatusCode, action: request =>
                {
                    actionIsCalled = true;
                    requestMessage = request;
                });
            else
                mock.SetupHttpResponseMessage(request => request.RequestUri == new Uri(requestUri), responseStatusCode);

            using (var httpClient = new HttpClient(mock.Object))
            {
                responseMessage = await httpClient.SendAsync(mockedRequestMessage);
            }

            // assert
            Assert.Null(responseMessage.Content);
            Assert.Equal(responseStatusCode, responseMessage.StatusCode);
            if (withAction) Assert.True(actionIsCalled);
            if (withAction) Assert.Equal(requestUri, requestMessage.RequestUri.AbsoluteUri);
            mock.Verify();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetupHttpResponseMessage_WithCustomizedRequest_WithContent(bool withAction)
        {
            // arrange
            const string requestUri = "http://www.site.com.br/";
            var mockedRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var responseContent = new StringContent("");
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(request => request.RequestUri == new Uri(requestUri), responseContent: responseContent, action: request =>
                {
                    actionIsCalled = true;
                    requestMessage = request;
                });
            else
                mock.SetupHttpResponseMessage(request => request.RequestUri == new Uri(requestUri), responseContent: responseContent);

            using (var httpClient = new HttpClient(mock.Object))
            {
                responseMessage = await httpClient.SendAsync(mockedRequestMessage);
            }

            // assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            Assert.Equal(responseContent, responseMessage.Content);
            if (withAction) Assert.True(actionIsCalled);
            if (withAction) Assert.Equal(requestUri, requestMessage.RequestUri.AbsoluteUri);
            mock.Verify();
        }

        [Theory]
        [InlineData(false, HttpStatusCode.BadGateway)]
        [InlineData(true, HttpStatusCode.Created)]
        public async Task SetupHttpResponseMessage_WithCustomizedRequest_WithHttpStatusCodeAndContent(bool withAction, HttpStatusCode responseStatusCode)
        {
            // arrange
            const string requestUri = "http://www.site.com.br/";
            var mockedRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var responseContent = new StringContent("");
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var actionIsCalled = false;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage responseMessage;

            // act
            if (withAction)
                mock.SetupHttpResponseMessage(request => request.RequestUri == new Uri(requestUri), responseStatusCode, responseContent: responseContent, action: request =>
                {
                    actionIsCalled = true;
                    requestMessage = request;
                });
            else
                mock.SetupHttpResponseMessage(request => request.RequestUri == new Uri(requestUri), responseStatusCode, responseContent: responseContent);

            using (var httpClient = new HttpClient(mock.Object))
            {
                responseMessage = await httpClient.SendAsync(mockedRequestMessage);
            }

            // assert
            Assert.Equal(responseStatusCode, responseMessage.StatusCode);
            Assert.Equal(responseContent, responseMessage.Content);
            if (withAction) Assert.True(actionIsCalled);
            if (withAction) Assert.Equal(requestUri, requestMessage.RequestUri.AbsoluteUri);
            mock.Verify();
        }

        #endregion

        [Fact]
        public void CreateHttpClientMock()
        {
            // arrange
            const string baseAddress = "http://www.site.com.br/";
            var mock = new Mock<HttpMessageHandler>();

            // act
            var client = mock.CreateHttpClientMock(baseAddress);

            // assert
            Assert.Equal(baseAddress, client.BaseAddress.AbsoluteUri);
            var messageHandlerField = typeof(HttpClient).BaseType.GetField("_handler", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            Assert.Equal(mock.Object, messageHandlerField.GetValue(client));
        }
    }
}
