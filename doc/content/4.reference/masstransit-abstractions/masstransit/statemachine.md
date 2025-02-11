---

title: StateMachine

---

# StateMachine

Namespace: MassTransit

A state machine definition

```csharp
public interface StateMachine : IVisitable, IProbeSite
```

Implements [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite)

## Properties

### **Name**

The name of the state machine (defaults to the state machine type name)

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Events**

The events defined in the state machine

```csharp
public abstract IEnumerable<Event> Events { get; }
```

#### Property Value

[IEnumerable\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **States**

The states defined in the state machine

```csharp
public abstract IEnumerable<State> States { get; }
```

#### Property Value

[IEnumerable\<State\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **InstanceType**

The instance type associated with the state machine

```csharp
public abstract Type InstanceType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Initial**

The initial state of a new state machine instance

```csharp
public abstract State Initial { get; }
```

#### Property Value

[State](../masstransit/state)<br/>

### **Final**

The final state of a state machine instance

```csharp
public abstract State Final { get; }
```

#### Property Value

[State](../masstransit/state)<br/>

## Methods

### **GetEvent(String)**

Returns the event requested

```csharp
Event GetEvent(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Event](../masstransit/event)<br/>

### **GetState(String)**

Returns the state requested

```csharp
State GetState(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[State](../masstransit/state)<br/>

### **NextEvents(State)**

The valid events that can be raised during the specified state

```csharp
IEnumerable<Event> NextEvents(State state)
```

#### Parameters

`state` [State](../masstransit/state)<br/>
The state to query

#### Returns

[IEnumerable\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
An enumeration of valid events

### **IsCompositeEvent(Event)**

Returns true if the event is or is used by a composite event

```csharp
bool IsCompositeEvent(Event event)
```

#### Parameters

`event` [Event](../masstransit/event)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
