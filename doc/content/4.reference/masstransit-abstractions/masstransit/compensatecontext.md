---

title: CompensateContext

---

# CompensateContext

Namespace: MassTransit

```csharp
public interface CompensateContext : CourierContext, ConsumeContext<RoutingSlip>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

Implements [CourierContext](../masstransit/couriercontext), [ConsumeContext\<RoutingSlip\>](../masstransit/consumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Result**

Set the compensation result, which completes the activity

```csharp
public abstract CompensationResult Result { get; set; }
```

#### Property Value

[CompensationResult](../masstransit/compensationresult)<br/>

## Methods

### **Compensated()**

The compensation was successful

```csharp
CompensationResult Compensated()
```

#### Returns

[CompensationResult](../masstransit/compensationresult)<br/>

### **Compensated(Object)**

The compensation was successful

```csharp
CompensationResult Compensated(object values)
```

#### Parameters

`values` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The variables to be updated on the routing slip

#### Returns

[CompensationResult](../masstransit/compensationresult)<br/>

### **Compensated(IDictionary\<String, Object\>)**

The compensation was successful

```csharp
CompensationResult Compensated(IDictionary<string, object> variables)
```

#### Parameters

`variables` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
The variables to be updated on the routing slip

#### Returns

[CompensationResult](../masstransit/compensationresult)<br/>

### **Failed()**

The compensation failed

```csharp
CompensationResult Failed()
```

#### Returns

[CompensationResult](../masstransit/compensationresult)<br/>

### **Failed(Exception)**

The compensation failed with the specified exception

```csharp
CompensationResult Failed(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[CompensationResult](../masstransit/compensationresult)<br/>
