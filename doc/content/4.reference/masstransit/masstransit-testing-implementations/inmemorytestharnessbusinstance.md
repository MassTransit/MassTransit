---

title: InMemoryTestHarnessBusInstance

---

# InMemoryTestHarnessBusInstance

Namespace: MassTransit.Testing.Implementations

```csharp
public class InMemoryTestHarnessBusInstance : IBusInstance, IReceiveEndpointConnector
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryTestHarnessBusInstance](../masstransit-testing-implementations/inmemorytestharnessbusinstance)<br/>
Implements [IBusInstance](../masstransit-transports/ibusinstance), [IReceiveEndpointConnector](../masstransit/ireceiveendpointconnector)

## Properties

### **Harness**

```csharp
public InMemoryTestHarness Harness { get; }
```

#### Property Value

[InMemoryTestHarness](../masstransit-testing/inmemorytestharness)<br/>

### **Name**

```csharp
public string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InstanceType**

```csharp
public Type InstanceType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Bus**

```csharp
public IBus Bus { get; }
```

#### Property Value

[IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **BusControl**

```csharp
public IBusControl BusControl { get; }
```

#### Property Value

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **HostConfiguration**

```csharp
public IHostConfiguration HostConfiguration { get; }
```

#### Property Value

[IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

## Constructors

### **InMemoryTestHarnessBusInstance(InMemoryTestHarness, IBusRegistrationContext)**

```csharp
public InMemoryTestHarnessBusInstance(InMemoryTestHarness testHarness, IBusRegistrationContext busRegistrationContext)
```

#### Parameters

`testHarness` [InMemoryTestHarness](../masstransit-testing/inmemorytestharness)<br/>

`busRegistrationContext` [IBusRegistrationContext](../masstransit/ibusregistrationcontext)<br/>

## Methods

### **Connect\<TRider\>(IRiderControl)**

```csharp
public void Connect<TRider>(IRiderControl riderControl)
```

#### Type Parameters

`TRider`<br/>

#### Parameters

`riderControl` [IRiderControl](../../masstransit-abstractions/masstransit-transports/iridercontrol)<br/>

### **GetRider\<TRider\>()**

```csharp
public TRider GetRider<TRider>()
```

#### Type Parameters

`TRider`<br/>

#### Returns

TRider<br/>

### **ConnectReceiveEndpoint(IEndpointDefinition, IEndpointNameFormatter, Action\<IBusRegistrationContext, IReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure)
```

#### Parameters

`definition` [IEndpointDefinition](../../masstransit-abstractions/masstransit/iendpointdefinition)<br/>

`endpointNameFormatter` [IEndpointNameFormatter](../../masstransit-abstractions/masstransit/iendpointnameformatter)<br/>

`configure` [Action\<IBusRegistrationContext, IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>

### **ConnectReceiveEndpoint(String, Action\<IBusRegistrationContext, IReceiveEndpointConfigurator\>)**

```csharp
public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure)
```

#### Parameters

`queueName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configure` [Action\<IBusRegistrationContext, IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

#### Returns

[HostReceiveEndpointHandle](../../masstransit-abstractions/masstransit/hostreceiveendpointhandle)<br/>
