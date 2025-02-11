---

title: HandlerConfigurator<TMessage>

---

# HandlerConfigurator\<TMessage\>

Namespace: MassTransit.Configuration

Connects a handler to the inbound pipe of the receive endpoint

```csharp
public class HandlerConfigurator<TMessage> : IHandlerConfigurator<TMessage>, IConsumeConfigurator, IHandlerConfigurationObserverConnector, IPipeConfigurator<ConsumeContext<TMessage>>, IReceiveEndpointSpecification, ISpecification
```

#### Type Parameters

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandlerConfigurator\<TMessage\>](../masstransit-configuration/handlerconfigurator-1)<br/>
Implements [IHandlerConfigurator\<TMessage\>](../../masstransit-abstractions/masstransit/ihandlerconfigurator-1), [IConsumeConfigurator](../../masstransit-abstractions/masstransit/iconsumeconfigurator), [IHandlerConfigurationObserverConnector](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserverconnector), [IPipeConfigurator\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1), [IReceiveEndpointSpecification](../../masstransit-abstractions/masstransit/ireceiveendpointspecification), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Constructors

### **HandlerConfigurator(MessageHandler\<TMessage\>, IHandlerConfigurationObserver)**

```csharp
public HandlerConfigurator(MessageHandler<TMessage> handler, IHandlerConfigurationObserver observer)
```

#### Parameters

`handler` [MessageHandler\<TMessage\>](../../masstransit-abstractions/masstransit/messagehandler-1)<br/>

`observer` [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)<br/>

## Methods

### **AddPipeSpecification(IPipeSpecification\<ConsumeContext\<TMessage\>\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TMessage>> specification)
```

#### Parameters

`specification` [IPipeSpecification\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1)<br/>

### **ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver)**

```csharp
public ConnectHandle ConnectHandlerConfigurationObserver(IHandlerConfigurationObserver observer)
```

#### Parameters

`observer` [IHandlerConfigurationObserver](../../masstransit-abstractions/masstransit/ihandlerconfigurationobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **Configure(IReceiveEndpointBuilder)**

```csharp
public void Configure(IReceiveEndpointBuilder builder)
```

#### Parameters

`builder` [IReceiveEndpointBuilder](../../masstransit-abstractions/masstransit-configuration/ireceiveendpointbuilder)<br/>
