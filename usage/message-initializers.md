# Initializing interface message contracts

MassTransit continues to encourage and support the use of interfaces for message contracts, and initializers make it easy to produce interface messages.

## An object of values

`Send`, `Publish`, and most of the methods that behave in similar ways (scheduling, responding to requests, etc.) all support passing an object of _values_ which is used to set the properties on the specified interface. A simple example is shown below.

Consider this example message contract to submit an order.

```csharp
public interface SubmitOrder
{
    Guid OrderId { get; }
    DateTime OrderDate { get; }
    string OrderNumber { get; }
    decimal OrderAmount { get; }
}
```

To send this message to an endpoint:

```csharp
await endpoint.Send<SubmitOrder>(new
{
    OrderId = NewId.NextGuid(),
    OrderDate = DateTime.UtcNow,
    OrderNumber = "18001",
    OrderAmount = 123.45m
});
```

The anonymous object is loosely typed, the properties are matched by name, and there is an extensive set of type conversions that may occur to obtain match the types defined by the interface. Most numeric, string, and date/time conversations are supported, as well as several advanced conversions (including variables, and asynchronous `Task<T>` results).

Collections, including arrays, lists, and dictionaries, are broadly supported, including the conversion of list elements, as well as dictionary keys and values. For instance, a dictionary of (int,decimal) could be converted on the fly to (long, string) using the default format conversions.

Nested objects are also supported, for instance, if a property was of type `Address` and another anonymous object was created (or any type whose property names match the names of the properties on the message contract), those properties would be set on the message contract.

## Headers

Header values can be specified in the anonymous object using a double-underscore (pronounced 'dunder' apparently) property name. For instance, to set the message time-to-live, specify a property with the duration. Remember, any value that can be converted to a `TimeSpan` works!

```csharp
public interface GetOrderStatus
{
    Guid OrderId { get; }
}

var response = await requestClient.GetResponse<OrderStatus>(new 
{
    __TimeToLive = 15000, // 15 seconds, or in this case, 15000 milliseconds
    OrderId = orderId,
});
```

> actually, that's a bad example since the request client already sets the message expiration, but you, get, the, point.

To add a custom header value, a special property name format is used. In the name, underscores are converted to dashes, and double underscores are converted to underscores. In the following example:

```csharp
var response = await requestClient.GetResponse<OrderStatus>(new 
{
    __Header_X_B3_TraceId = zipkinTraceId,
    __Header_X_B3_SpanId = zipkinSpanId,
    OrderId = orderId,
});
```

This would include set the headers used by open tracing (or Zipkin, as shown above) as part of the request message so the service could share in the span/trace. In this case, `X-B3-TraceId` and `X-B3-SpanId` would be added to the message envelope, and depending upon the transport, copied to the transport headers as well.

## Variables

MassTransit also supports variables, which are special types added to the anonymous object. Following the example above, the initialization could be changed to use variables for the `OrderId` and `OrderDate`. Variables are consistent throughout the message creation, using the same variable multiple times returns the value. For instance, the Id created to set the _OrderId_ would be the same used to set the _OrderId_ in each item.

```csharp
public interface OrderItem
{
    Guid OrderId { get; }
    string ItemNumber { get; }
}

public interface SubmitOrder
{
    Guid OrderId { get; }
    DateTime OrderDate { get; }
    string OrderNumber { get; }
    decimal OrderAmount { get; }
    OrderItem[] OrderItems { get; }
}

await endpoint.Send<SubmitOrder>(new
{
    OrderId = InVar.Id,
    OrderDate = InVar.Timestamp,
    OrderNumber = "18001",
    OrderAmount = 123.45m,
    OrderItems = new[]
    {
        new { OrderId = InVar.Id, ItemNumber = "237" },
        new { OrderId = InVar.Id, ItemNumber = "762" }
    }
});
```

## Awaiting Task results

Message initializers are now asynchronous, which makes it possible to do some pretty cool things, including waiting for Task input properties to complete and use the result to initialize the property. An example is shown below.

```csharp
public interface OrderUpdated
{
    Guid CorrelationId { get; }
    DateTime Timestamp { get; }
    Guid OrderId { get; }
    Customer Customer { get; }
}

public async Task<CustomerInfo> LoadCustomer(Guid orderId)
{
    // work happens up in here
}

await context.Publish<OrderUpdated>(new
{
    InVar.CorrelationId,
    InVar.Timestamp,
    OrderId = context.Message.OrderId,
    Customer = LoadCustomer(context.Message.OrderId)
});
```

The property initializer will wait for the task result and then use it to initialize the property (converting all the types, etc. as it would any other object).

> While it is of course possible to await the call to `LoadCustomer`, properties are initialized in parallel, and thus, allowing the initializer to await the Task can result in better overall performance. Your mileage may vary, however.



