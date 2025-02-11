---

title: IPipeConfigurator<TContext>

---

# IPipeConfigurator\<TContext\>

Namespace: MassTransit

Configures a pipe with specifications

```csharp
public interface IPipeConfigurator<TContext>
```

#### Type Parameters

`TContext`<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<TContext\>)**

Adds a pipe specification to the pipe configurator at the end of the chain

```csharp
void AddPipeSpecification(IPipeSpecification<TContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<TContext\>](../masstransit-configuration/ipipespecification-1)<br/>
The pipe specification to add
