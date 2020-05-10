# Requests

Request/response is a common pattern in application development, where a component sends a request to a service and continues once the response is received. In a distributed system, this can increase the latency of an application since the service may be hosted in another process, on another machine, or may even be a remote service in another network. While in many cases it is best to avoid request/response use in distributed applications, particularly when the request is a command, it is often necessary and preferred over more complex solutions.

Fortunately for .NET developers, C# with TPL makes it easier to program applications that call services asynchronously. By using *Tasks* and the *async* and *await* keywords, developers can write procedural code and avoid the complex use of callbacks and handlers. Additionally, multiple asynchronous requests can be executed at once, reducing the overall execution time to that of the longest request.

### Message Contracts

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

## Request Consumer

In order for the request to return anything, it needs to be handled. Handling requests is done by using normal consumers. The only difference is that such consumer needs to send a response back.

For the aforementioned message contracts, the request handler can look like this:

```csharp
public class CheckOrderStatusConsumer : 
    IConsumer<CheckOrderStatus>
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
        
        await context.RespondAsync<OrderStatusResult>(new 
        {
            OrderId = order.Id,
            order.Timestamp,
            order.StatusCode,
            order.StatusText
        });
    }
}
```

The response will be sent back to the requestor. In case the exception is thrown, MassTransit will create a `Fault<CheckOrderStatus>` message and send it back to the requestor. The requestor address is available in the consume context of the request message as `context.ResponseAddress`.

## Request Client

Most interactions of the request/response nature consist of four elements: the request arguments, the response values, exception handling, and the time to wait for a response. The .NET framework gives us one additional element, a `CancellationToken` which can prematurely cancel waiting for the response.

In MassTransit, the request client is composed of two parts, a client factory, and a request client. The client factory is created from the bus, or a connected endpoint, and has the interface below (some overloads are omitted, but you get the idea).

```csharp
public interface IClientFactory 
{
    IRequestClient<T> CreateRequestClient<T>(ConsumeContext context, Uri destinationAddress, RequestTimeout timeout);

    IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout);

    RequestHandle<T> CreateRequest<T>(T request, Uri destinationAddress, CancellationToken cancellationToken, RequestTimeout timeout);

    RequestHandle<T> CreateRequest<T>(ConsumeContext context, T request, Uri destinationAddress, CancellationToken cancellationToken, RequestTimeout timeout);
}
```

As shown, the client factory can create a request client, or it can create a request directly. There are advantages to each approach, although it's typically best to create a request client and use it if possible. If a consumer is sending the request, a new client should be created for each message (and is handled automatically if you're using a dependency injection container and the container registration methods).

To create a client factory, call `bus.CreateClientFactory` or `host.CreateClientFactory` -- after the bus has been started.

The request client can be used to create requests (returning a `RequestHandle<T>`, which must be disposed after the request completes) or it can be used directly to send a request and get a response (asynchronously, of course).

> Using `Create` returns a request handle, which can be used to set headers and other attributes of the request before it is sent.

```csharp
public interface IRequestClient<TRequest>
    where TRequest : class
{
    RequestHandle<TRequest> Create(TRequest request, CancellationToken cancellationToken, RequestTimeout timeout);

    Task<Response<T>> GetResponse<T>(TRequest request, CancellationToken cancellationToken, RequestTimeout timeout);
}
```

### Sending a Request

To create a request client, and use it to make a standalone request (not from a consumer, API controller, etc.):

```csharp
var serviceAddress = new Uri("rabbitmq://localhost/check-order-status");
var client = bus.CreateRequestClient<CheckOrderStatus>(serviceAddress);

var response = await client.GetResponse<OrderStatusResult>(new { OrderId = id});
```

The response type, `Response<OrderStatusResult>` includes the _MessageContext_ from when the response was received, providing access to the message properties (such as `response.ConversationId`) and headers (`response.Headers`). 

To create a request, and add a header to the `SendContext`, use the _Create_ method which returns a _RequestHandle_ and then set the header using an execute filter as shown.

```csharp
using (var request = client.Create(new { OrderId = id})
{
    request.UseExecute(x => x.Headers.Set("custom-header", "some-value"));

    var response = await request.GetResponse<OrderStatusResult>();
}
```

Calling the `GetResponse` method triggers the request to be sent, after which the caller awaits the response. To add additional response types, see below for the tuple syntax, or just add multiple `GetResponse` methods, passing _false_ for the _readyToSend_ parameter.

See below for examples of how to use the request client in different contexts.

::: warning IMPORTANT
MassTransit uses a temporary non-durable queue and has a consumer to handle responses. This temporary queue only get configured and created when you _start the bus_. If you forget to start the bus in your application code, the request client will fail with a timeout, waiting for a response.
:::

## Container Configuration

To register a request client for use with ASP.NET Core, it is recommended to use the [MassTransit.ExtensionsDependencyInjection](https://www.nuget.org/packages/MassTransit.Extensions.DependencyInjection/) NuGet package. It can be used to setup ASP.NET to use MassTransit, and has registration methods to ensure consumers and request clients are properly registered.

To configure the request client in ASP.NET, use the registration extension shown.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<CheckOrderStatusConsumer>();

        x.AddBus(context => Bus.Factory.CreateUsingInMemory(cfg =>
        {
            cfg.ConfigureEndpoints(context);
        }));

        x.AddRequestClient<CheckOrderStatus>();
    });
}
```

Once registered, a controller can use the client via a constructor dependency.

```csharp
public class RequestController :
    Controller
{
    IRequestClient<CheckOrderStatus> _client;

    public RequestController(IRequestClient<CheckOrderStatus> client)
    {
        _client = client;
    }

    public async Task<ActionResult> Get(string id)
    {
        var response = await _client.GetResponse<OrderStatusResult>(new {OrderId = id});

        return View(response.Message);
    }
}
```

The controller method will send the command, and return the view once the result has been received.

## Multiple Requests

If there were multiple requests to be performed, it is easy to wait on all results at the same time, benefiting from the concurrent operation.

```csharp
public class RequestController : 
    Controller
{
    IRequestClient<RequestA> _clientA;
    IRequestClient<RequestB> _clientB;

    public RequestController(IRequestClient<RequestA> clientA, IRequestClient<RequestB> clientB)
    {
        _clientA = clientA;
        _clientB = clientB;
    }

    public async Task<ActionResult> Get()
    {
        var resultA = _clientA.GetResponse(new RequestA());
        var resultB = _clientB.GetResponse(new RequestB());

        await Task.WhenAll(resultA, resultB);

        var a = await resultA;
        var b = await resultB;

        var model = new Model(a.Message, b.Message);

        return View(model);
    }
}
```

The power of concurrency, for the win!

## Multiple Response Types

Another powerful feature with the request client is the ability support multiple (such as positive and negative) result types. For example, adding an `OrderNotFound` response type to the consumer as shown eliminates throwing an exception since a missing order isn't really a fault.

```csharp
public class CheckOrderStatusConsumer : 
    IConsumer<CheckOrderStatus>
{
    public async Task Consume(ConsumeContext<CheckOrderStatus> context)
    {
        var order = await _orderRepository.Get(context.Message.OrderId);
        if (order == null)
            await context.RespondAsync<OrderNotFound>(context.Message);
        else        
            await context.RespondAsync<OrderStatusResult>(new 
            {
                OrderId = order.Id,
                order.Timestamp,
                order.StatusCode,
                order.StatusText
            });
    }
}
```

The client can now wait for multiple response types (in this case, two) by using a little tuple magic.

```csharp
var (statusResponse,notFoundResponse) = await client.GetResponse<OrderStatusResult, OrderNotFound>(new { OrderId = id});

// both tuple values are Task<Response<T>>, need to find out which one completed
if(statusResponse.IsCompletedSuccessfully)
{
    var orderStatus = await statusResponse;
    // do something
}
else
{
    var notFound = await notFoundResponse;
    // do something else
}

```

This cleans up the processing, an eliminates the need to catch a `RequestFaultException`.