---

title: IMessageSource<T>

---

# IMessageSource\<T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface IMessageSource<T>
```

#### Type Parameters

`T`<br/>

## Properties

### **Sinks**

```csharp
public abstract IEnumerable<IMessageSink<T>> Sinks { get; }
```

#### Property Value

[IEnumerable\<IMessageSink\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

## Methods

### **Connect(IMessageSink\<T\>, String)**

```csharp
ConnectHandle Connect(IMessageSink<T> sink, string routingKey)
```

#### Parameters

`sink` [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1)<br/>

`routingKey` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
