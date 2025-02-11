---

title: IBuildPipeConfigurator<TContext>

---

# IBuildPipeConfigurator\<TContext\>

Namespace: MassTransit.Configuration

```csharp
public interface IBuildPipeConfigurator<TContext> : IPipeConfigurator<TContext>, ISpecification
```

#### Type Parameters

`TContext`<br/>

Implements [IPipeConfigurator\<TContext\>](../masstransit/ipipeconfigurator-1), [ISpecification](../masstransit/ispecification)

## Methods

### **Build()**

Builds the pipe, applying any initial specifications to the front of the pipe

```csharp
IPipe<TContext> Build()
```

#### Returns

[IPipe\<TContext\>](../masstransit/ipipe-1)<br/>
