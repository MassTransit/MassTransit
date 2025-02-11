---

title: IMessageQueue<TContext, T>

---

# IMessageQueue\<TContext, T\>

Namespace: MassTransit.Transports.Fabric

```csharp
public interface IMessageQueue<TContext, T> : IMessageSink<T>, IProbeSite
```

#### Type Parameters

`TContext`<br/>

`T`<br/>

Implements [IMessageSink\<T\>](../masstransit-transports-fabric/imessagesink-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Properties

### **Name**

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **ConnectMessageReceiver(TContext, IMessageReceiver\<T\>)**

```csharp
TopologyHandle ConnectMessageReceiver(TContext nodeContext, IMessageReceiver<T> receiver)
```

#### Parameters

`nodeContext` TContext<br/>

`receiver` [IMessageReceiver\<T\>](../masstransit-transports-fabric/imessagereceiver-1)<br/>

#### Returns

[TopologyHandle](../masstransit-transports-fabric/topologyhandle)<br/>
