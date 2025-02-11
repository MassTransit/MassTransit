---

title: TriggerEvent

---

# TriggerEvent

Namespace: MassTransit.SagaStateMachine

```csharp
public class TriggerEvent : Event, IVisitable, IProbeSite, IComparable<Event>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TriggerEvent](../masstransit-sagastatemachine/triggerevent)<br/>
Implements [Event](../../masstransit-abstractions/masstransit/event), [IVisitable](../../masstransit-abstractions/masstransit/ivisitable), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IComparable\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1)

## Properties

### **Name**

```csharp
public string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Constructors

### **TriggerEvent(String)**

```csharp
public TriggerEvent(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **Accept(StateMachineVisitor)**

```csharp
public void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../../masstransit-abstractions/masstransit/statemachinevisitor)<br/>

### **Probe(ProbeContext)**

```csharp
public void Probe(ProbeContext context)
```

#### Parameters

`context` [ProbeContext](../../masstransit-abstractions/masstransit/probecontext)<br/>

### **CompareTo(Event)**

```csharp
public int CompareTo(Event other)
```

#### Parameters

`other` [Event](../../masstransit-abstractions/masstransit/event)<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Equals(TriggerEvent)**

```csharp
public bool Equals(TriggerEvent other)
```

#### Parameters

`other` [TriggerEvent](../masstransit-sagastatemachine/triggerevent)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
