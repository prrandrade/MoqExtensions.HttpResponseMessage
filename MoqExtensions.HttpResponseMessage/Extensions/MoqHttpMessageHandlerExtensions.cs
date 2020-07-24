namespace MoqExtensions.HttpResponseMessage.Extensions
{
    using System;
    using System.Net.Http;
    using Moq;
    using Moq.Protected;

    public static class MoqHttpMessageHandlerExtensions
    {
        /// <summary>
        /// Mark the dispose method of
        /// </summary>
        /// <param name="mock"></param>
        public static void MarkDisposeAsVerifiable(this Mock<HttpMessageHandler> mock)
        {
            mock.Protected()
                .Setup("Dispose", ItExpr.IsAny<bool>())
                .Verifiable();
        }

        /// <summary>
        /// Creates a HttpClient that uses the passed Mock<![CDATA[<HttpMessageHandler>]]>
        /// </summary>
        /// <param name="mock">The Mock<![CDATA[<HttpMessageHandler>]]> that the new HttpClient will use</param>
        /// <param name="baseAddress">The optional HttpClient base address</param>
        /// <returns>A new HttpClient with a customized HttpMessageHandler</returns>
        public static HttpClient CreateHttpClientMock(this Mock<HttpMessageHandler> mock, string baseAddress = null)
        {
            var httpClient = new HttpClient(mock.Object);

            if (baseAddress != null)
                httpClient.BaseAddress = new Uri(baseAddress);

            return httpClient;
        }
    }
}
