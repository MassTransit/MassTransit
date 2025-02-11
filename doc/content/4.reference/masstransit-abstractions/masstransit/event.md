---

title: Event

---

# Event

Namespace: MassTransit

```csharp
public interface Event : IVisitable, IProbeSite, IComparable<Event>
```

Implements [IVisitable](../masstransit/ivisitable), [IProbeSite](../masstransit/iprobesite), [IComparable\<Event\>](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1)

## Properties

### **Name**

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
