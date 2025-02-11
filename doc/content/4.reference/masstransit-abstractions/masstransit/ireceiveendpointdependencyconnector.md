---

title: IReceiveEndpointDependencyConnector

---

# IReceiveEndpointDependencyConnector

Namespace: MassTransit

```csharp
public interface IReceiveEndpointDependencyConnector
```

## Methods

### **AddDependency(IReceiveEndpointDependency)**

Add receive endpoint dependency. Endpoint will be started when dependency is Ready

```csharp
void AddDependency(IReceiveEndpointDependency dependency)
```

#### Parameters

`dependency` [IReceiveEndpointDependency](../masstransit-transports/ireceiveendpointdependency)<br/>
