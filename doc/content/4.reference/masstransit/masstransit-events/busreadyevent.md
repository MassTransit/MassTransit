---

title: BusReadyEvent

---

# BusReadyEvent

Namespace: MassTransit.Events

```csharp
public class BusReadyEvent : BusReady
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusReadyEvent](../masstransit-events/busreadyevent)<br/>
Implements [BusReady](../../masstransit-abstractions/masstransit/busready)

## Properties

### **Bus**

```csharp
public IBus Bus { get; }
```

#### Property Value

[IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **Host**

```csharp
public HostReady Host { get; }
```

#### Property Value

[HostReady](../../masstransit-abstractions/masstransit/hostready)<br/>

## Constructors

### **BusReadyEvent(HostReady, IBus)**

```csharp
public BusReadyEvent(HostReady host, IBus bus)
```

#### Parameters

`host` [HostReady](../../masstransit-abstractions/masstransit/hostready)<br/>

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>
