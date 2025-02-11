---

title: State

---

# State

Namespace: MassTransit

```csharp
public interface State : IVisitable, IProbeSite, IComparable<State>
```

Implements [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite), [IComparable\<State\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1)

## Properties

### **Name**

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Enter**

Raised when the state is entered

```csharp
public abstract Event Enter { get; }
```

#### Property Value

[Event](../masstransit/event)<br/>

### **Leave**

Raised when the state is about to be left

```csharp
public abstract Event Leave { get; }
```

#### Property Value

[Event](../masstransit/event)<br/>

### **BeforeEnter**

Raised just before the state is about to change to a new state

```csharp
public abstract Event<State> BeforeEnter { get; }
```

#### Property Value

[Event\<State\>](../masstransit/event-1)<br/>

### **AfterLeave**

Raised just after the state has been left and a new state is selected

```csharp
public abstract Event<State> AfterLeave { get; }
```

#### Property Value

[Event\<State\>](../masstransit/event-1)<br/>
