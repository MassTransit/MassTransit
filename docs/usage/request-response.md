# Crafting a request/response conversation
<!-- TOC -->

- [Creating the message contracts](#creating-the-message-contracts)
- [Handling requests](#handling-requests)
- [Creating the message request client](#creating-the-message-request-client)
- [Using the request client in ASP.NET MVC](#using-the-request-client-in-aspnet-mvc)
- [Composing multiple requests](#composing-multiple-requests)

<!-- /TOC -->
Request/response is a common pattern in application development, where a component sends a request to a service and
continues once the response is received. In a distributed system, this can increase the latency of an application
since the service may be hosted in another process, on another machine, or may even be a remote service in another
network. While in many cases it is best to avoid request/response use in distributed applications, particularly when
the request is a command, it is often necessary and preferred over more complex solutions.

Fortunately for .NET developers, C# with TPL makes it easier to program applications
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

## Handling requests

In order for the request to return anything, it needs to be handled. Handling requests
is done by using normal consumers. The only difference is that such consumer needs to send a response back.

For the aforementioned message contracts, the request handler can look like this:

```csharp
public class CheckOrderStatusConsumer : IConsumer<CheckOrderStatus>
{
    readonly IOrderRepository _orderRepository;

    public CheckOrderStatusConsumer(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Consume(ConsumeContext<CheckOrderStatus> context)
    {
        var order = await _orderRepository.Get(context.Message.OrderId);
        if (order == null)
            throw new InvalidOperationException("Order not found");
        
        await context.RespondAsync<OrderStatusResult>(
            new 
            {
                OrderId = order.Id,
                Timestamp = order.Timestamp,
                StatusCode = order.StatusCode,
                StatusText = order.StatusText
            }
        )
    }
}
```

The response will be sent back to the requestor. In case the exception is thrown, 
MassTransit will create a `Fault<CheckOrderStatus>` message and send it back to the
requestor. The requestor address is available in the consume context of the 
request message as `context.ResponseAddress`.

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
and endpoints. The configuration of the application can define the endpoints and connections and register them in
a dependency injection container, keeping the configuration complexity at the outer edge of the application.

To create a request client, the provided `MessageRequestClient` can be used.

```csharp
var address = new Uri("loopback://localhost/order_status_check");
var requestTimeout = TimeSpan.FromSeconds(30);

IRequestClient<CheckOrderStatus, OrderStatusResult> client =
    new MessageRequestClient<CheckOrderStatus, OrderStatusResult>(bus, address, requestTimeout);
```

Once created, the request client instance can be used to perform the request:

```csharp
var result = await _client.Request<CheckOrderStatus>(new {OrderId = id});
```

The syntax is significantly cleaner than dealing with message object, consumer contexts, responses,
etc. And since async/await and messaging are both about asynchronous programming, it's a natural fit.

> **Important:** MassTransit uses a temporary non-durable queue and has a consumer to handle responses. This temporary queue only get configured and created when you _start the bus_. If you forget to start the bus in your application code, the request client will fail with a timeout, waiting for a response.

## Using the request client in ASP.NET MVC

The request client instance can be registered with the dependency resolver using the `IRequestClient`
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
        var result = await _client.Request<CheckOrderStatus>(new {OrderId = id});

        return View(result);
    }
}
```

The controller method will send the command, and return the view once the result has been received.

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
