MoqExtensions.HttpResponseMessage
=============================

# Summary

- [Introduction](#introduction)
- [Problem](#problem)
- [Tradicional Solution](#tradicional-solution)
- [New Approach](#new-approach)

# Introduction

This project intends to facilitate the creation of mocked **HttpResponseMessage** objects given an already created **HttpMessageHandler** mocks using the **Moq** library. The objective here is to abstract the used logic when creating a **HttpResponseMessage** given a specific or generic **HttpRequestMessage**.

Nuget package: https://www.nuget.org/packages/MoqExtensions.HttpResponseMessage/



# Problem

Consuming an API is something common today and a .NET Developer normally uses a HttpClient to achieve it. The problem happens when a Unit Test needs to be written, because of two problems:

- The `HttpClient` commonly is created inside the method (or the class) that will consume an API. This approach creates an undesired dependency, which goes against the **Dependency Inversion Principle** (the last of the five S.O.L.I.D. principles). Fortunately, the .NET 5.0 framework contains the `IHttpClientFactory` interface that can be injected and can be used to create a `HttpClient`.

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



# Tradicional Solution

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

This approach works flawlessly, but imagine redoing all this logic for each unit test that covers an API request/response. The **MoqExtensions.HttpResponseMessage** project intend to simplify it.



# New approach

You still need two mocks, one for the `HttpMessageHandler` and one for the `IHttpClientFactory` (that will be injected). But the preparations for these mocks are way simpler:

```csharp
var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
var httpClientFactoryMock = new Mock<IHttpClientFactory>();
httpClientFactoryMock.SetupHttpClientFactory(httpMessageHandlerMock);
httpMessageHandkerMock.MarkDisposeAsVerifiable();
```

With the extension method `SetupHttpClientFactory`, you can configure the customized `Mock<HttpMessageHandler>` to be used when the injected `Mock<IHttpClientFactory>()` is called inside an unit test. And if the `HttpClient` is called on a disposable way, the extension method `MarkDisposeAsVerifiable` can be used to verify if the `HttpClient` is, in fact, being disposable. You simply do not need to worry anymore about a `HttpClient`, all the configurations are applying to the Mock<IHttpClientFactory>()`. But how?

For example, if you need to mock a request that always return the OK http status code, that's simple:

```csharp
MockRequestConfiguration
    .New()
    .BuildAt(httpMessageHandlerMock);
```

If you need to mock a request for a speficic address with a specific response (a bad request status code, for example), that's also simple:

```csharp
MockRequestConfiguration
    .New()
    .WithRequestAddress("http://www.site.com/")
    .WithResponseStatusCode(HttpStatusCode.BadRequest)
    .BuildAt(httpMessageHandlerMock);
```

But if you need to toe mocks for requests with different methods (a POST method and a GET method, for example), you need two configurations (please note that the configurations do not need to be applied in a specific order, using a [Fluent interface](https://en.wikipedia.org/wiki/Fluent_interface) inspired pattern):

```csharp
MockRequestConfiguration
    .New()
    .WithRequestMethod(HttpMethod.Post)
    .WithResponseStatusCode(HttpStatusCode.BadRequest)
    .WithRequestAddress("http://www.site.com/")    
    .BuildAt(httpMessageHandlerMock);
```

```csharp
MockRequestConfiguration
    .New()    
    .WithResponseStatusCode(HttpStatusCode.Accepted)    
    .WithRequestAddress("http://www.site.com/")   
    .WithRequestMethod(HttpMethod.Get)
    .BuildAt(httpMessageHandlerMock);
```

But, what if you need to specify the request and response objects? That is also easy (please note that the request and response objects as today are automatically converted to and from JSON objects):

```csharp
public class DummyObjectRequest {}
public class DummyObjectResponse {}

MockRequestConfiguration<DummyObjectRequest, DummyObjectResponse>
    .New()    
    .WithResponseStatusCode(HttpStatusCode.Accepted)    
    .WithRequestAddress("http://www.site.com/")   
    .WithRequestMethod(HttpMethod.Get)
    .BuildAt(httpMessageHandlerMock);
```

Finally, if you need to retrieve the original request and/or request content during a unit test... well, that's also easy:

```csharp
public class DummyObjectRequest {}
public class DummyObjectResponse {}

HttpRequestMessage requestMessage = null;
DummyObjectRequest requestMessageContent = null;

MockRequestConfiguration<DummyObjectRequest, DummyObjectResponse>
    .New()
    .RetrieveRequestMessageAt(r => requestMessage = r)
    .RetrieveRequestMessageContentAt(r => requestMessageContent = r)
    .WithRequestMethod(HttpMethod.Get)
    .WithResponseStatusCode(HttpStatusCode.Accepted)    
    .WithRequestAddress("http://www.site.com/")
    .BuildAt(httpMessageHandlerMock);
```

