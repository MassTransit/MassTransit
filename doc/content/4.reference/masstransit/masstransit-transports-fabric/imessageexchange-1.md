---

title: IMessageExchange<T>

---

# IMessageExchange\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface IMessageExchange<T> : IMessageSink<T>, IProbeSite, IMessageSource<T>
```

#### Type Parameters

`T`<br/>

Implements [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IMessageSource\<T\>](../masstransit-transports-fabric/imessagesource-1)

## Properties

### **Name**

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
