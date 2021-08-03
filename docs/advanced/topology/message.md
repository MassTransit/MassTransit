# Message Topology

Message types are extensively leveraged in MassTransit, so making it easy to configure how those message types are used by topology seemed obvious.

## Entity Name Formatters

### Message Type Entity Name Formatting

MassTransit has built-in defaults for naming messaging entities (these are things like exchanges, topics, etc.). The defaults can be overridden as well. For instance, to change the topic name used by a message, just do it!

```csharp
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Message<OrderSubmitted>(x =>
    {
        x.SetEntityName("omg-we-got-one");
    });
});
```

It's also possible to create a message-specific entity name formatter, by implementing `IMessageEntityNameFormatter<T>` and specifying it during configuration.

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

Bus.Factory.CreateUsingRabbitMq(cfg =>
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

Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Message<OrderSubmitted>(x =>
    {
        x.SetEntityNameFormatter(new FancyNameFormatter(cfg.MessageTopology.EntityNameFormatter));;
    });
});
```

## Attributes

### EntityName Attribute

_EntityName_ is an optional attribute used to override the default entity name for a message type. If present, the entity name will be used when creating the topic or exchange for the message.

```cs
[EntityName("order-submitted")]
public interface LegacyOrderSubmittedEvent
{
}
```

### ConfigureConsumeTopology Attribute

_ConfigureConsumeTopology_ is an optional attribute that may be specified on a message type to indicate whether the topic or exchange for the message type should be created and subscribed to the queue when consumed on a receive endpoint.

```cs
[ConfigureConsumeTopology(false)]
public interface DeleteRecord
{
}
```

### ExcludeFromTopology Attribute

_ExcludeFromTopology_ is an optional attribute that may be specified on a message type to indicate whether the topic or exchange for the message type should be created when publishing an implementing type or sub-type. In the example below, publishing the `ReformatHardDrive` command would not create the `ICommand` topic or exchange on the message broker.

```cs
[ExcludeFromTopology]
public interface ICommand
{
}

public interface ReformatHardDrive :
    ICommand
{
}
```

To avoid using the property, the publish topology can be configured along with the bus:

```cs
...UsingRabbitMq((context,cfg) =>
{
    cfg.Publish<ICommand>(p => p.Exclude = true);
});
```
