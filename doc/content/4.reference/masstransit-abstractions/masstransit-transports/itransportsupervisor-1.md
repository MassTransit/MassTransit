---

title: ITransportSupervisor<T>

---

# ITransportSupervisor\<T\>

Namespace: MassTransit.Transports

```csharp
public interface ITransportSupervisor<T> : ISupervisor<T>, ISupervisor, IAgent, IAgent<T>, IPipeContextSource<T>, IProbeSite
```

#### Type Parameters

`T`<br/>

Implements [ISupervisor\<T\>](../masstransit/isupervisor-1), [ISupervisor](../masstransit/isupervisor), [IAgent](../masstransit/iagent), [IAgent\<T\>](../masstransit/iagent-1), [IPipeContextSource\<T\>](../masstransit/ipipecontextsource-1), [IProbeSite](../masstransit/iprobesite)

## Properties

### **ConsumeStopping**

```csharp
public abstract CancellationToken ConsumeStopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

### **SendStopping**

```csharp
public abstract CancellationToken SendStopping { get; }
```

#### Property Value

[CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

## Methods

### **AddConsumeAgent\<TAgent\>(TAgent)**

```csharp
void AddConsumeAgent<TAgent>(TAgent agent)
```

#### Type Parameters

`TAgent`<br/>

#### Parameters

`agent` TAgent<br/>

### **AddSendAgent\<TAgent\>(TAgent)**

```csharp
void AddSendAgent<TAgent>(TAgent agent)
```

#### Type Parameters

`TAgent`<br/>

#### Parameters

`agent` TAgent<br/>
