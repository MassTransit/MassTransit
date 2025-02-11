---

title: IEndpointConfigurationObserver

---

# IEndpointConfigurationObserver

Namespace: MassTransit

```csharp
public interface IEndpointConfigurationObserver
```

## Methods

### **EndpointConfigured\<T\>(T)**

Called when an endpoint is configured

```csharp
void EndpointConfigured<T>(T configurator)
```

#### Type Parameters

`T`<br/>
The receive endpoint configurator type

#### Parameters

`configurator` T<br/>
