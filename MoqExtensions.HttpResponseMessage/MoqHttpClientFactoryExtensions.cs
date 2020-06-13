namespace MoqExtensions.HttpResponseMessage
{
    using System.Net.Http;
    using Microsoft.Extensions.Options;
    using Moq;

    public static class MoqHttpClientFactoryExtensions
    {
        /// <summary>
        /// Setup an Mock<![CDATA[<IHttpClientFactory>]]> so that it creates a customized HttpClient
        /// </summary>
        /// <param name="mockClientFactory">The Mock<![CDATA[<IHttpClientFactory>]]> that will be setup</param>
        /// <param name="httpClient">The HttpClient that will be returned</param>
        public static void SetupHttpClientFactory(this Mock<IHttpClientFactory> mockClientFactory, HttpClient httpClient)
        {
            mockClientFactory
                .Setup(x => x.CreateClient(Options.DefaultName))
                .Returns(httpClient);
        }

        /// <summary>
        /// Setup an Mock<![CDATA[<IHttpClientFactory>]]> so that it creates a customized HttpClient
        /// </summary>
        /// <param name="mockClientFactory">The Mock<![CDATA[<IHttpClientFactory>]]> that will be setup</param>
        /// <param name="mockMessageHandler">The Mock<![CDATA[<HttpMessageHandler>]]> that will be used when creating a HttpClient</param>
        /// <param name="baseAddress">The base address that the HttpClient will use</param>
        /// <returns>The customized HttpClient that will be used with the passed Mock<![CDATA[<IHttpClientFactory>]]></returns>
        public static HttpClient SetupHttpClientFactory(this Mock<IHttpClientFactory> mockClientFactory, Mock<HttpMessageHandler> mockMessageHandler, string baseAddress)
        {
            var httpClient = mockMessageHandler.CreateHttpClientMock(baseAddress);
            mockClientFactory.SetupHttpClientFactory(httpClient);
            return httpClient;
        }

        /// <summary>
        /// Setup an Mock<![CDATA[<IHttpClientFactory>]]> so that it creates a customized HttpClient
        /// </summary>
        /// <param name="mockClientFactory">The Mock<![CDATA[<IHttpClientFactory>]]> that will be setup</param>
        /// <param name="httpClient">The HttpClient that will be returned</param>
        /// <param name="httpClientName">Thew customized HttpClient name</param>
        public static void SetupHttpClientFactory(this Mock<IHttpClientFactory> mockClientFactory, HttpClient httpClient, string httpClientName)
        {
            mockClientFactory
                .Setup(x => x.CreateClient(httpClientName))
                .Returns(httpClient);
        }

        /// <summary>
        /// Setup an Mock<![CDATA[<IHttpClientFactory>]]> so that it creates a customized HttpClient
        /// </summary>
        /// <param name="mockClientFactory">The Mock<![CDATA[<IHttpClientFactory>]]> that will be setup</param>
        /// <param name="mockMessageHandler">The Mock<![CDATA[<HttpMessageHandler>]]> that will be used when creating a HttpClient</param>
        /// <param name="baseAddress">The base address that the HttpClient will use</param>
        /// <param name="httpClientName">Thew customized HttpClient name</param>
        /// <returns>The customized HttpClient that will be used with the passed Mock<![CDATA[<IHttpClientFactory>]]></returns>
        public static HttpClient SetupHttpClientFactory(this Mock<IHttpClientFactory> mockClientFactory, Mock<HttpMessageHandler> mockMessageHandler, string baseAddress, string httpClientName)
        {
            var httpClient = mockMessageHandler.CreateHttpClientMock(baseAddress);
            mockClientFactory.SetupHttpClientFactory(httpClient, httpClientName);
            return httpClient;
        }
    }
}
