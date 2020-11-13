namespace MoqExtensions.HttpResponseMessage.UnitTest.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Reflection;
    using Moq;
    using Moq.Protected;
    using Xunit;
    using static HttpResponseMessage.Extensions.MoqHttpMessageHandlerExtensions;

    [ExcludeFromCodeCoverage]
    public class MoqHttpMessageHandlerExtensionsTests
    {
        [Fact]
        public void MarkDisposeAsVerifiable_DisposableScenario()
        {
            // arrange
            var mock = new Mock<HttpMessageHandler>();
            mock.MarkDisposeAsVerifiable();

            // act
            using (var httpClient = new HttpClient(mock.Object)) { }

            // assert
            mock.Verify();
            mock.Protected().Verify("Dispose", Times.Once(), ItExpr.IsAny<bool>());
        }

        [Fact]
        public void MarkDisposeAsVerifiable_NotDisposableScenario()
        {
            // arrange
            var mock = new Mock<HttpMessageHandler>();
            mock.MarkDisposeAsVerifiable();

            // act
            var httpClient = new HttpClient(mock.Object);

            // assert
            mock.Protected().Verify("Dispose", Times.Never(), ItExpr.IsAny<bool>());
        }

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
