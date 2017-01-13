# Crafting a request/response conversation

Request/response is a common pattern in application development, where a component sends a request to a service and
continues once the response is received. In a distributed system, this can increase the latency of an application
since the service many be hosted in another process, on another machine, or may even be a remote service in another
network. While in many cases it is best to avoid request/response use in distributed applications, particularly when
the request is a command, it is often necessary and preferred over more complex solutions.

Fortunately for .NET developers, C# 5.0 introduced the async/await keywords, making it easier to program applications
that call services asynchronously. By using *Tasks* and the *async* and *await* keywords, developers can write
procedural code and avoid the complex use of callbacks and handlers. Additionally, multiple asynchronous requests can
be executed at once, reducing the overall execution time to that of the longest request.

## Creating the message contracts

To get started, the message contracts need to be created. In this example, an order status check is being created.

```csharp
public interface CheckOrderStatus
{
    string OrderId { get; }
}

public interface OrderStatusResult
{
    string OrderId { get; }
    DateTime Timestamp { get; }
    short StatusCode { get; }
    string StatusText { get; }
}
```

## Creating the message request client

Most interactions of the request/response nature consist of four elements: the request arguments, the response values,
exception handling, and the time to wait for a response. The .NET framework gives us one additional element, a
`CancellationToken` which can prematurely cancel waiting for the response. The request client optimizes these elements
into an easy-to-use interface:

```csharp
public interface IRequestClient<TRequest, TResponse>
{
    Task<TResponse> Request(TRequest request, CancellationToken cancellationToken);
}
```

The interface is simple, a single method that accepts the request and returns a Task that can be awaited. The interface
declares the request and response types, making it useful for dependency management using dependency injection. In fact,
by using the request client, an application can be completely free of any MassTransit concerns such as message contexts
and endpoints. The configuration of the application can defined the endpoints and connections and register them in
a dependency injection container, keeping the configuration complexity at the outer edge of the application.

To create a request client, the provided `MessageRequestClient` can be used.

```csharp
var address = new Uri("loopback://localhost/order_status_check");
var requestTimeout = TimeSpan.FromSeconds(30);

IRequestClient<CheckOrderStatus, OrderStatusResult> client =
    new MessageRequestClient<CheckOrderStatus, OrderStatusResult>(bus, address, requestTimeout);
```

Once created, the request client instance can be registered with the dependency resolver using the `IRequestClient`
interface type. Once registered, a controller can use the client via a constructor dependency.

```csharp
public class RequestController : Controller
{
    IRequestClient<CheckOrderStatus, OrderStatusResult> _client;

    public RequestController(IRequestClient<CheckOrderStatus, OrderStatusResult> client)
    {
        _client = client;
    }

    public async Task<ActionResult> Get(string id)
    {
        var command = new CheckOrderStatus
        {
            OrderId = id
        };

        var result = await _client.Request(command);

        return View(result);
    }
}
```

The controller method will send the command, and return the view once the result has been received.
The syntax is significantly cleaner than dealing with message object, consumer contexts, responses,
etc. And since async/await and messaging are both about asynchronous programming, it's a natural fit.

## Composing multiple requests

If there were multiple requests to be performed, it is easy to wait on all results at the same time,
benefiting from the concurrent operation.

```csharp
public class RequestController : Controller
{
    IRequestClient<RequestA, ResultA> _clientA;
    IRequestClient<RequestB, ResultB> _clientB;

    public RequestController(IRequestClient<RequestA, ResultA> clientA, IRequestClient<RequestB, ResultB> clientB)
    {
        _clientA = clientA;
        _clientB = clientB;
    }

    public async Task<ActionResult> Get()
    {
        var requestA = new RequestA();
        Task<ResultA> resultA = _clientA.Request(requestA);

        var requestB = new RequestB();
        Task<ResultB> resultB = _clientB.Request(requestB);

        await Task.WhenAll(resultA, resultB);

        var model = new Model(resultA.Result, resultB.Result);

        return View(model);
    }
}
```

The power of concurrency, for the win!
