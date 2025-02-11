---

title: IReceiveEndpointDependentConnector

---

# IReceiveEndpointDependentConnector

Namespace: MassTransit

```csharp
public interface IReceiveEndpointDependentConnector
```

## Methods

### **AddDependent(IReceiveEndpointDependent)**

Add the dependent to receive endpoint. Receive endpoint will be stopped when dependent is Completed

```csharp
void AddDependent(IReceiveEndpointDependent dependent)
```

#### Parameters

`dependent` [IReceiveEndpointDependent](../masstransit-transports/ireceiveendpointdependent)<br/>
