---

title: BindPipeSpecification<TLeft, TRight>

---

# BindPipeSpecification\<TLeft, TRight\>

Namespace: MassTransit.Configuration

```csharp
public class BindPipeSpecification<TLeft, TRight> : IPipeSpecification<TLeft>, ISpecification, IBindConfigurator<TLeft, TRight>, IPipeConfigurator<BindContext<TLeft, TRight>>
```

#### Type Parameters

`TLeft`<br/>

`TRight`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BindPipeSpecification\<TLeft, TRight\>](../masstransit-configuration/bindpipespecification-2)<br/>
Implements [IPipeSpecification\<TLeft\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IBindConfigurator\<TLeft, TRight\>](../masstransit/ibindconfigurator-2), [IPipeConfigurator\<BindContext\<TLeft, TRight\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)

## Constructors

### **BindPipeSpecification(IPipeContextSource\<TRight, TLeft\>)**

```csharp
public BindPipeSpecification(IPipeContextSource<TRight, TLeft> source)
```

#### Parameters

`source` [IPipeContextSource\<TRight, TLeft\>](../../masstransit-abstractions/masstransit/ipipecontextsource-2)<br/>
