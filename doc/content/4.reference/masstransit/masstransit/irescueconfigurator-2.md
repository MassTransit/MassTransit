---

title: IRescueConfigurator<TContext, TRescue>

---

# IRescueConfigurator\<TContext, TRescue\>

Namespace: MassTransit

```csharp
public interface IRescueConfigurator<TContext, TRescue> : IExceptionConfigurator, IPipeConfigurator<TRescue>
```

#### Type Parameters

`TContext`<br/>

`TRescue`<br/>

Implements [IExceptionConfigurator](../../masstransit-abstractions/masstransit/iexceptionconfigurator), [IPipeConfigurator\<TRescue\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Properties

### **ContextPipe**

Configure a filter on the context pipe, versus the rescue pipe

```csharp
public abstract IPipeConfigurator<TContext> ContextPipe { get; }
```

#### Property Value

[IPipeConfigurator\<TContext\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
