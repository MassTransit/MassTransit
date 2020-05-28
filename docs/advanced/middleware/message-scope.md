# Message Scope

A message lifetime scope is available within a pipeline. This can be useful for resolving per-message services for the lifetime of the message.

For a consume pipeline this must be made explicit by calling `UseServiceScope()` during the configuration:

::: tip NOTE
By placing `UseServiceScope()` before the first occurance of `UseFilter()` will ensure the message lifetime scope is available from the first filter onwards.
:::

```csharp
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    // start of consume pipeline

    cfg.UseServiceScope(busContext.Container);// message lifetime scope starts here

    // define the rest of the consume pipeline
    cfg.UseFilter(new CorrelationIdConsumeFilter());
    cfg.UseFilter(new UserIdentificationConsumeFilter());

    cfg.ReceiveEndpoint("customer_update_queue", e =>
    {
        // other configuration
    });
});
```

In a consume filter the message lifetime scope can be retrieved from the context payload cache; for example:

```csharp
public class CorrelationIdConsumeFilter : IFilter<ConsumeContext>
{
    // This filter gets the correlation-id from inbound bus messages.

    public CorrelationIdConsumeFilter()
    {
        // Note: singleton filter cannot take scoped dependencies in the constructor
        // See 'Send() below
    }

    public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        // retrieve the lifetime scope service provider (for MS DI in this example)
        var serviceProvider = context.GetPayload<IServiceProvider>();

        // resolve some scoped service
        var correlationIdManager = serviceProvider.GetRequiredService<ICorrelationIdManager>();

        correlationIdManger.SetId(context.CorrelationId);
    }
}
```

For a send pipeline the message lifetime scope is available automatically:

```csharp
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ConfigureSend(pipe =>
    {
        pipe.UseFilter(new CorrelationIdSendFilter());
        pipe.UseFilter(new UserIdentificationSendFilter());
    });
});
```
However, for this to work, use of the endpoint resolver for the `Send()` call is mandatory: 

::: warning
If you use the `IBus` abstraction with `Send()` this will not create a message lifetime scope in the `SendContext`.
:::

```csharp
public class CustomerUpdater<TMessage> where TMessage : class
{
    private readonly ISendEndpointProvider _endpointProvider;

    public CustomerUpdater(ISendEndpointProvider endpointProvider) => _endpointProvider = endpointProvider;

    public async Task SendMessage(TMessage message)
    {
        var endpoint = await _endpointProvider.GetSendEndpoint(new Uri("queue:customer_update_queue"));
        await endpoint.Send(message);
    }
}
```
In a send filter the nessage lifetime scope can be retrieved from the context payload cache; for example:

```csharp
public class CorrelationIdSendFilter : IFilter<SendContext>
{
    // This filter sets a correlation-id for outbound bus messages.

    public CorrelationIdSendFilter()
    {
        // Note: singleton filter cannot take scoped dependencies in the constructor
        // See 'Send() below
    }

    public async Task Send(SendContext context, IPipe<SendContext> next)
    {
        // retrieve the lifetime scope service provider (for MS DI in this example)
        var serviceProvider = context.GetPayload<IServiceProvider>();

        // resolve some scoped service
        var correlationIdManager = serviceProvider.GetRequiredService<ICorrelationIdManager>();

        context.CorrelationId = correlationIdManager.GetId();
    }
}
```