# MoqExtensions.HttpResponseMessage

### Introduction

This project intends to facilitate the creation of mocked **HttpResponseMessage** objects given an already created **HttpMessageHandler** mocks using the **Moq** library. The objective here is to abstract the used logic when creating a **HttpResponseMessage** given a specific or generic **HttpRequestMessage**.



### Problem

Consuming an API is something common today and a .NET Developer normally uses a HttpClient to achieve it. The problem happens when a Unit Test needs to be written, because of two problems:

- The `HttpClient` commonly is created inside the method (or the class) that will consume an API. This approach creates an undesired dependency, which goes against the **Dependency Inversion Principle** (the last of the five S.O.L.I.D. principles). Fortunately, the .NET Core framework contains the `IHttpClientFactory` interface that can be injected and can be used to create a `HttpClient`.

  ```csharp
  public class ExampleClass {
      private readonly IHttpClientFactory _httpClientFactoryMock;
      
      public ExampleClass(IHttpClientFactory httpClientFactory)
      {
          _httpClientFactory = httpClientFactory;
      }
      
      public SomeMethod() {
          using var client = _httpClientFactory.CreateClient();
          // code continues
      }
  }
  ```

- Even using an `IHttpClientFactory` to dynamically create a `HttpClient`, this object does not implement any interface, so it´s difficult to mock. The solution is to Mock an underlying `HttpMessageHandler` that is used within a `HttpClient` for the API communications (requests and responses).

### Traditional solution

The ideal scenario can be imagined using a `HttpRequestMessage` to construct a request and passing this request for the `HttpClient`. Like this, for example:

```csharp
public SomeMethod() {
    using var request = new HttpRequestMessage(HttpMethod.Get, "www.someaddress.com/api");
    using var client = _httpClientFactory.CreateClient();
    var response = await client.SendAsync(request);
    // code continues    
}
```

Given the fact that the `HttpClient` uses the protected method `SendTo` of a `HttpMessageHandler` for the request/response handling, we can mock an `IHttpClientFactory` to return a customized `HttpClient` that uses a mocked `HttpMessageHandler`, so we can mock requests and responses. But is easier said than done. Look at an example of this approach, mocking a `HttpResponseMessage` with no content and the 200 status code.

```csharp
var handlerMock = new Mock<HttpMessageHandler>();

HttpRequestMessage originalRequest = null;

handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
    "SendAsync",
    ItExpr.IsAny<HttpRequestMessage>(),
    ItExpr.IsAny<CancellationToken>()
)
.Callback<HttpRequestMessage, CancellationToken>((request, token) => originalRequest = request)
.ReturnsAsync(new HttpResponseMessage
{
   StatusCode = HttpStatusCode.OK
})
.Verifiable();
```

As you can see, we are using the `Protected()` method of the mock so we can mock the **SendAsync** method of the `HttpMessageHandler`. We also are using the `Callback` method of the mock so we can retrieve the original `HttpRequestMessage` that will be sent - and this is the only opportunity to do so! What we need to do is to create a mock of `IHttpClientFactory` (this mock can be injected, as we saw earlier) and configure it to use a customized `HttpClient` - one that uses the `Mock<HttpMessageHandler>` that was configured earlier.

```csharp
var httpClientFactoryMock = new Mock<IHttpClientFactory>();

var httpClient = new HttpClient(handlerMock.Object)
{
	BaseAddress = new Uri(BaseAddress)
};

httpClientFactoryMock
	.Setup(x => x.CreateClient(It.IsAny<string>()))
	.Returns(httpClient);
```

This approach works flawlessly, but imagine redoing all this logic for each unit test that covers an API request/response. The MoqExtensions.HttpResponseMessage project intend to simplify it.



### MoqExtensions.HttpResponseMessage approach

Given the fact that you will need two mocks, one for the `HttpMessageHandler` and one for the `IHttpClientFactory` (that will be injected), the same example can be rewritten like this:

```csharp
var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
var httpClientFactoryMock = new Mock<IHttpClientFactory>();

HttpRequestMessage request = null;
handlerMock.SetupHttpResponseMessage(HttpStatusCode.OK), originalRequest => request = originalRequest);
httpClientFactoryMock.SetupHttpClientFactory(handlerMock, "www.someaddress.com/api");
```

First, we use the extension method `SetupHttpResponseMessage` to set up a request with no content in this case. The last parameter is an optional action with the sent request as a parameter so you can retrieve it for asserts, for example. Then we use the extension method `SetupHttpClientFactory` so the  `IHttpClientFactory` mock can return a `HttpClient` already configured to use the customized `HttpMessageHandler` mock. Way less work per unit test!



### MoqExtensions.HttpResponseMessage methods

For a `HttpMessageHandler` mock, the following extension methods are added using **MoqExtensions.HttpResponseMessage**:

- `SetupHttpResponseMessage(Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(HttpStatusCode responseStatusCode, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(HttpStatusCode responseStatusCode, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`

- `SetupHttpResponseMessage(HttpRequestMessage httpRequestMessage, Action<HttpRequestMessage> action = null, bool verifiableDispose = true, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(HttpRequestMessage httpRequestMessage, HttpStatusCode responseStatusCode, Action<HttpRequestMessage> action = null, bool verifiableDispose = true, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(HttpRequestMessage httpRequestMessage, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(HttpRequestMessage httpRequestMessage, HttpStatusCode responseStatusCode, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`

- `SetupHttpResponseMessage(Expression<Func<HttpRequestMessage, bool>> httpRequestMessageExpression, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(Expression<Func<HttpRequestMessage, bool>> httpRequestMessageExpression, HttpStatusCode responseStatusCode, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(Expression<Func<HttpRequestMessage, bool>> httpRequestMessageExpression, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`
- `SetupHttpResponseMessage(Expression<Func<HttpRequestMessage, bool>> httpRequestMessageExpression, HttpStatusCode responseStatusCode, HttpContent responseContent, Action<HttpRequestMessage> action = null, bool verifiableDispose = true)`

- `HttpClient CreateHttpClientMock(string baseAddress)`

  

For a `IHttpClientFactory` mock, the following extensions methods are added using **MoqExtensions.HttpResponseMessage**:

- `SetupHttpClientFactory(HttpClient httpClient)`
- `HttpClient SetupHttpClientFactory(Mock<HttpMessageHandler> mockMessageHandler, string baseAddress)`

- `void SetupHttpClientFactory(HttpClient httpClient, string httpClientName)`
- `HttpClient SetupHttpClientFactory(Mock<HttpMessageHandler> mockMessageHandler, string baseAddress, string httpClientName)`


### Next Steps

- [X] Version 1.0.0 on Nuget 
- [X] Add custom requests when creating a mock
- [X] Add parameter for testing when a HttpClient is Disposable (parameter `verifiableDispose`)

