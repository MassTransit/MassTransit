---

title: IPublishPipeConfigurator

---

# IPublishPipeConfigurator

Namespace: MassTransit

```csharp
public interface IPublishPipeConfigurator : IPipeConfigurator<PublishContext>, IPublishPipeSpecificationObserverConnector
```

Implements [IPipeConfigurator\<PublishContext\>](../masstransit/ipipeconfigurator-1), [IPublishPipeSpecificationObserverConnector](../masstransit-configuration/ipublishpipespecificationobserverconnector)

## Methods

### **AddPipeSpecification(IPipeSpecification\<SendContext\>)**

Adds a type-specific pipe specification to the consume pipe

```csharp
void AddPipeSpecification(IPipeSpecification<SendContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<SendContext\>](../masstransit-configuration/ipipespecification-1)<br/>

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

### **AddPipeSpecification\<T\>(IPipeSpecification\<PublishContext\<T\>\>)**

Adds a type-specific pipe specification to the consume pipe

```csharp
void AddPipeSpecification<T>(IPipeSpecification<PublishContext<T>> specification)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`specification` [IPipeSpecification\<PublishContext\<T\>\>](../masstransit-configuration/ipipespecification-1)<br/>
