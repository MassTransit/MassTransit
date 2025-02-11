---

title: IBindConfigurator<TLeft, TRight>

---

# IBindConfigurator\<TLeft, TRight\>

Namespace: MassTransit

Configures a binding using the specified pipe context source

```csharp
public interface IBindConfigurator<TLeft, TRight> : IPipeConfigurator<BindContext<TLeft, TRight>>
```

#### Type Parameters

`TLeft`<br/>

`TRight`<br/>

Implements [IPipeConfigurator\<BindContext\<TLeft, TRight\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Properties

### **ContextPipe**

Configure a filter on the context pipe, versus the bound pipe

```csharp
public abstract IPipeConfigurator<TLeft> ContextPipe { get; }
```

#### Property Value

[IPipeConfigurator\<TLeft\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
