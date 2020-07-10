namespace MoqExtensions.HttpResponseMessage.UnitTest
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using Moq;
    using Xunit;

    public class DummyObject
    {
        public int DummyValue { get; set; }
    }

    public class MockRequestConfiguratorTest
    {
        [Fact]
        public void WithRequestMethod()
        {
            // arrange
            var method = HttpMethod.Get;
            var r = new MockRequestConfiguration<string, string>();

            // act
            r = r.WithRequestMethod(method);

            // assert
            Assert.Equal(method, r.RequestMethod);
        }

        [Fact]
        public void WithRequestAddress()
        {
            // arrange
            const string address = "http://www.somesite.com/";
            var r = new MockRequestConfiguration<string, string>();

            // act
            r = r.WithRequestAddress(address);

            // assert
            Assert.Equal(address, r.RequestUri.OriginalString);
        }

        [Fact]
        public void RetrieveRequestMessageAt()
        {
            // arrange
            void Action(HttpRequestMessage rq) { }
            Action(new HttpRequestMessage());

            var r = new MockRequestConfiguration<string, string>();

            // act
            r = r.RetrieveRequestMessageAt(Action);

            // assert
            Assert.Equal(Action, r.OriginalRequestAction);
        }

        [Fact]
        public void RetrieveRequestMessageContentAt()
        {
            // arrange
            static void Action(string s) { }
            Action("");

            var r = new MockRequestConfiguration<string, string>();

            // act
            r = r.RetrieveRequestMessageContentAt(Action);

            // assert
            Assert.Equal(Action, r.OriginalRequestContentAction);
        }

        [Fact]
        public void WithResponseStatusCode()
        {
            // arrange
            const HttpStatusCode status = HttpStatusCode.Accepted;
            var r = new MockRequestConfiguration<string, string>();

            // act
            r = r.WithResponseStatusCode(status);

            // assert
            Assert.Equal(status, r.ResponseStatusCode);
        }

        [Fact]
        public void WithResponseStatusCode_Default()
        {
            // arrange
            var r = new MockRequestConfiguration<string, string>();

            // assert
            Assert.Equal(HttpStatusCode.OK, r.ResponseStatusCode);
        }

        [Fact]
        public void WithResponseContent_string()
        {
            // arrange
            const string responseContent = "content";
            var r = new MockRequestConfiguration<string, string>();

            // act
            r = r.WithResponseContent(responseContent);

            // assert
            Assert.Equal(responseContent, r.ResponseContent);
        }

        [Fact]
        public void WithResponseContent_object()
        {
            // arrange
            var obj = new DummyObject();
            var r = new MockRequestConfiguration<string, DummyObject>();

            // act
            r = r.WithResponseContent(obj);

            // assert
            Assert.Equal(obj, r.ResponseContent);
        }

        [Fact]
        public async void BuildAt()
        {
            // arrange
            var mock = new Mock<HttpMessageHandler>();
            const HttpStatusCode responseStatusCode = HttpStatusCode.Created;
            const string requestAddress = "http://www.somesite.com/";
            var originalMessageContent = new DummyObject { DummyValue = 1 };

            // act
            HttpRequestMessage requestMessage = null;
            DummyObject requestMessageContent = null;

            MockRequestConfiguration.New<DummyObject, DummyObject>()
                .RetrieveRequestMessageAt(r => requestMessage = r)
                .RetrieveRequestMessageContentAt(r => requestMessageContent = r)
                .WithResponseStatusCode(responseStatusCode)
                .WithRequestMethod(HttpMethod.Post)
                .WithRequestAddress(requestAddress)
                .BuildAt(mock);

            using var httpClient = new HttpClient(mock.Object);
            var response = await httpClient.PostAsync(requestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json"));

            // assert
            Assert.Equal(responseStatusCode, response.StatusCode);
            Assert.Equal(requestAddress, requestMessage.RequestUri.AbsoluteUri);
            Assert.Equal(originalMessageContent.DummyValue, requestMessageContent.DummyValue);
        }
    }
}
