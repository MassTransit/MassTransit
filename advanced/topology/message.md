# Message Topology

Message types are extensively leveraged in MassTransit, so making it easy to configure how those message types are used by topology seemed obvious.

## Message Type Entity Name Formatting

MassTransit has built-in defaults for naming messaging entities (these are things like exchanges, topics, etc.). But we get it, you like options. With MassTransit v4, those options are now free to be used (and probably abused).

For instance, to change the topic name used by a message, just do it!

```csharp
Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.Message<OrderSubmitted>(x =>
    {
        x.SetEntityName("omg-we-got-one");
    });
});
```

It's also possible to create a message-specific entity name formatter, by implmenting `IMessageEntityNameFormatter<T>` and specifying it during configuration.

```csharp
class FancyNameFormatter<T> :
    IMessageEntityNameFormatter<T>
{
    public string FormatEntityName()
    {
        // seriously, please don't do this, like, ever.
        return type(T).Name.ToString();
    }
}

Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.Message<OrderSubmitted>(x =>
    {
        x.SetEntityNameFormatter(new FancyNameFormatter<OrderSubmitted>());
    });
});
```

It's also possible to replace the entity name formatter for the entire topology.

```csharp
class FancyNameFormatter<T> :
    IMessageEntityNameFormatter<T>
{
    public string FormatEntityName()
    {
        // seriously, please don't do this, like, ever.
        return type(T).Name.ToString();
    }
}

class FancyNameFormatter :
    IEntityNameFormatter
{
    public FancyNameFormatter(IEntityNameFormatter original)
    {
        _original = original;
    }

    public string FormatEntityName<T>()
    {
        if(T is OrderSubmitted)
            return "we-got-one";

        return _original.FormatEntityName<T>();
    }
}

Bus.Factory.CreateUsingRabbitMQ(..., cfg =>
{
    cfg.Message<OrderSubmitted>(x =>
    {
        x.SetEntityNameFormatter(new FancyNameFormatter(cfg.MessageTopology.EntityNameFormatter));;
    });
});
```
