namespace MoqExtensions.HttpResponseMessage.UnitTest
{
    using System;
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
        public void CreatingNewConfigurationWithRequest()
        {
            // act
            var result = MockRequestConfiguration.New<DummyObject>();

            // assert
            Assert.IsType<MockRequestConfiguration<DummyObject, object>>(result);
        }

        [Fact]
        public void CreatingNewConfigurationWithRequestAndResponse()
        {
            // act
            var result = MockRequestConfiguration.New<string, DummyObject>();

            // assert
            Assert.IsType<MockRequestConfiguration<string, DummyObject>>(result);
        }


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
        public async void BuildAt_WithResponseMethod_WithRequestMethod_WithRequestUri()
        {
            // arrange
            const string requestAddress = "http://www.somesite.com/";
            const string otherRequestAddress = "http://www.someothersite.com/";

            var mock = new Mock<HttpMessageHandler>();
            const HttpStatusCode responseStatusCode = HttpStatusCode.Created;
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

            // act correct case
            var response = await httpClient.PostAsync(requestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json"));

            // assert correct case
            Assert.Equal(responseStatusCode, response.StatusCode);
            Assert.Equal(requestAddress, requestMessage.RequestUri.AbsoluteUri);
            Assert.Equal(originalMessageContent.DummyValue, requestMessageContent.DummyValue);

            // act incorrect cases
            var exception1 = await Record.ExceptionAsync(async () => await httpClient.PutAsync(requestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json")));
            var exception2 = await Record.ExceptionAsync(async () => await httpClient.PostAsync(otherRequestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json")));

            // assert incorrect cases
            Assert.IsType<InvalidOperationException>(exception1);
            Assert.IsType<InvalidOperationException>(exception2);
        }

        [Fact]
        public async void BuildAt_WithResponseMethod()
        {
            // arrange
            const string requestAddress = "http://www.somesite.com/";
            const string otherRequestAddress = "http://www.someothersite.com/";

            var mock = new Mock<HttpMessageHandler>();
            const HttpStatusCode responseStatusCode = HttpStatusCode.Created;
            var originalMessageContent = new DummyObject { DummyValue = 1 };

            // act
            MockRequestConfiguration.New<DummyObject, DummyObject>()
                .WithResponseStatusCode(responseStatusCode)
                .WithRequestMethod(HttpMethod.Post)
                .BuildAt(mock);

            using var httpClient = new HttpClient(mock.Object);

            // act correct cases
            var response1 = await httpClient.PostAsync(requestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json"));
            var response2 = await httpClient.PostAsync(otherRequestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json"));

            // assert correct cases
            Assert.Equal(responseStatusCode, response1.StatusCode);
            Assert.Equal(responseStatusCode, response2.StatusCode);

            // act incorrect cases
            var exception1 = await Record.ExceptionAsync(async () => await httpClient.PutAsync(requestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json")));
            var exception2 = await Record.ExceptionAsync(async () => await httpClient.PutAsync(otherRequestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json")));

            // assert incorrect cases
            Assert.IsType<InvalidOperationException>(exception1);
            Assert.IsType<InvalidOperationException>(exception2);
        }

        [Fact]
        public async void BuildAt_WithRequestUri()
        {
            // arrange
            const string requestAddress = "http://www.somesite.com/";
            const string otherRequestAddress = "http://www.someothersite.com/";

            var mock = new Mock<HttpMessageHandler>();
            var originalMessageContent = new DummyObject { DummyValue = 1 };

            MockRequestConfiguration.New<DummyObject, DummyObject>()
                 .BuildAt(mock);

            using var httpClient = new HttpClient(mock.Object);

            // act correct cases
            var response1 = await httpClient.PostAsync(requestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json"));
            var response2 = await httpClient.PutAsync(requestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json"));
            var response3 = await httpClient.PostAsync(otherRequestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json"));
            var response4 = await httpClient.PutAsync(otherRequestAddress, new StringContent(JsonSerializer.Serialize(originalMessageContent), Encoding.UTF8, "application/json"));

            // assert correct cases
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response4.StatusCode);
        }

        [Fact]
        public async void BuildAt_WithStringResponseContent()
        {
            // arrange
            const string responseContent = "example";
            var mock = new Mock<HttpMessageHandler>();

            MockRequestConfiguration.New<string>()
                .WithResponseContent(responseContent)
                .BuildAt(mock);
            using var httpClient = new HttpClient(mock.Object);

            // act
            var response = await httpClient.GetAsync("http://www.somesite.com/");

            // assert
            Assert.Equal(responseContent, (await response.Content.ReadAsStringAsync()));
        }

        [Fact]
        public async void BuildAt_WithObjectResponseContent()
        {
            // arrange
            var responseContent = new DummyObject { DummyValue = 5 };
            var mock = new Mock<HttpMessageHandler>();

            MockRequestConfiguration.New<DummyObject>()
                .WithResponseContent(responseContent)
                .BuildAt(mock);
            using var httpClient = new HttpClient(mock.Object);

            // act
            var response = await httpClient.PostAsync("http://www.somesite.com/", new StringContent(JsonSerializer.Serialize(new DummyObject()), Encoding.UTF8, "application/json"));

            // assert
            Assert.Equal(responseContent.DummyValue, JsonSerializer.Deserialize<DummyObject>(await response.Content.ReadAsStringAsync()).DummyValue);
        }
    }
}
