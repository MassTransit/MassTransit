---

title: ISendPipeConfigurator

---

# ISendPipeConfigurator

Namespace: MassTransit

```csharp
public interface ISendPipeConfigurator : IPipeConfigurator<SendContext>, ISendPipeSpecificationObserverConnector
```

Implements [IPipeConfigurator\<SendContext\>](../masstransit/ipipeconfigurator-1), [ISendPipeSpecificationObserverConnector](../masstransit-configuration/isendpipespecificationobserverconnector)

## Methods

### **AddPipeSpecification\<T\>(IPipeSpecification\<SendContext\<T\>\>)**

Adds a type-specific pipe specification to the consume pipe

```csharp
void AddPipeSpecification<T>(IPipeSpecification<SendContext<T>> specification)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`specification` [IPipeSpecification\<SendContext\<T\>\>](../masstransit-configuration/ipipespecification-1)<br/>
