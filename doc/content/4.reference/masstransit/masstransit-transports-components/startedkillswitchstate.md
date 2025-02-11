---

title: StartedKillSwitchState

---

# StartedKillSwitchState

Namespace: MassTransit.Transports.Components

```csharp
public class StartedKillSwitchState : IKillSwitchState, IConsumeObserver, IProbeSite
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StartedKillSwitchState](../masstransit-transports-components/startedkillswitchstate)<br/>
Implements [IKillSwitchState](../masstransit-transports-components/ikillswitchstate), [IConsumeObserver](../../masstransit-abstractions/masstransit/iconsumeobserver), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite)

## Constructors

### **StartedKillSwitchState(IKillSwitch)**

```csharp
public StartedKillSwitchState(IKillSwitch killSwitch)
```

#### Parameters

`killSwitch` [IKillSwitch](../masstransit-transports-components/ikillswitch)<br/>

## Methods

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **PreConsume\<T\>(ConsumeContext\<T\>)**

```csharp
public Task PreConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **PostConsume\<T\>(ConsumeContext\<T\>)**

```csharp
public Task PostConsume<T>(ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ConsumeFault\<T\>(ConsumeContext\<T\>, Exception)**

```csharp
public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **LogThreshold()**

```csharp
public void LogThreshold()
```

### **Activate()**

```csharp
public void Activate()
```
