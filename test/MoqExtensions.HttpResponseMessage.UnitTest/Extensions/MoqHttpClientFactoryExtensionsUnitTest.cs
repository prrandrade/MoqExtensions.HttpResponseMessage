namespace MoqExtensions.HttpResponseMessage.UnitTest.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Reflection;
    using HttpResponseMessage.Extensions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class MoqHttpClientFactoryExtensionsUnitTest
    {
        [Fact]
        public void SetupHttpClientFactory_WithHttpClient()
        {
            // arrange
            var httpClient = new HttpClient();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            
            // act
            httpClientFactoryMock.SetupHttpClientFactory(httpClient);
            var resultHttpClient = httpClientFactoryMock.Object.CreateClient();

            // assert
            Assert.Equal(httpClient, resultHttpClient);
            httpClientFactoryMock.Verify(x => x.CreateClient(Options.DefaultName), Times.Once);
        }

        [Fact]
        public void SetupHttpClientFactory_WithMessageHandler_WithBaseAddress()
        {
            // arrange
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            const string baseAddress = "http://www.site.com.br/";

            // act
            var httpClient = httpClientFactoryMock.SetupHttpClientFactory(messageHandlerMock, baseAddress);
            var resultHttpClient = httpClientFactoryMock.Object.CreateClient();

            // assert
            Assert.Equal(httpClient, resultHttpClient);
            Assert.Equal(baseAddress, httpClient.BaseAddress.AbsoluteUri);
            var messageHandlerField = typeof(HttpClient).BaseType.GetField("_handler", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            Assert.Equal(messageHandlerMock.Object, messageHandlerField.GetValue(httpClient));
        }

        [Fact]
        public void SetupHttpClientFactory_WithHttpClient_WithCustomName()
        {
            // arrange
            var httpClient = new HttpClient();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var customName = Guid.NewGuid().ToString();
            
            // act
            httpClientFactoryMock.SetupHttpClientFactory(httpClient, customName);
            var resultHttpClient = httpClientFactoryMock.Object.CreateClient(customName);

            // assert
            Assert.Equal(httpClient, resultHttpClient);
            httpClientFactoryMock.Verify(x => x.CreateClient(customName), Times.Once);
        }

        [Fact]
        public void SetupHttpClientFactory_WithMessageHandler_WithBaseAddress_SetupHttpClientFactory_WithHttpClient_WithCustomName()
        {
            // arrange
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            const string baseAddress = "http://www.site.com.br/";
            var customName = Guid.NewGuid().ToString();

            // act
            var httpClient = httpClientFactoryMock.SetupHttpClientFactory(messageHandlerMock, customName, baseAddress);
            var resultHttpClient = httpClientFactoryMock.Object.CreateClient(customName);

            // assert
            Assert.Equal(httpClient, resultHttpClient);
            Assert.Equal(baseAddress, httpClient.BaseAddress.AbsoluteUri);
            var messageHandlerField = typeof(HttpClient).BaseType.GetField("_handler", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            Assert.Equal(messageHandlerMock.Object, messageHandlerField.GetValue(httpClient));
        }
    }
}
