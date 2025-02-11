---

title: IPipeBuilder<TContext>

---

# IPipeBuilder\<TContext\>

Namespace: MassTransit.Configuration

A pipe builder constructs a pipe by adding filter to the end of the chain, after
 while the builder completes the pipe/filter combination.

```csharp
public interface IPipeBuilder<TContext>
```

#### Type Parameters

`TContext`<br/>
The pipe context type

## Methods

### **AddFilter(IFilter\<TContext\>)**

Add a filter to the pipe after any existing filters

```csharp
void AddFilter(IFilter<TContext> filter)
```

#### Parameters

`filter` [IFilter\<TContext\>](../masstransit/ifilter-1)<br/>
The filter to add
