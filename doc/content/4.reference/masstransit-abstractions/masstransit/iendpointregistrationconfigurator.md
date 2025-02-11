---

title: IEndpointRegistrationConfigurator

---

# IEndpointRegistrationConfigurator

Namespace: MassTransit

```csharp
public interface IEndpointRegistrationConfigurator
```

## Properties

### **Name**

Set the endpoint name, overriding the default endpoint name formatter

```csharp
public abstract string Name { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Temporary**

True if the endpoint should be removed after the endpoint is stopped

```csharp
public abstract bool Temporary { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **PrefetchCount**

Only specify when required, use [IEndpointRegistrationConfigurator.ConcurrentMessageLimit](iendpointregistrationconfigurator#concurrentmessagelimit) first and
 only specific a [IEndpointRegistrationConfigurator.PrefetchCount](iendpointregistrationconfigurator#prefetchcount) when the default is not appropriate

```csharp
public abstract Nullable<int> PrefetchCount { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConcurrentMessageLimit**

The maximum number of concurrent messages processing at one time on the endpoint. Is
 used to configure the transport efficiently.

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **ConfigureConsumeTopology**

Defaults to true, which connects topics/exchanges/etc. to the endpoint queue at the broker.
 If set to false, no broker topology is configured (automatically set to false for courier
 activities since [RoutingSlip](../masstransit-courier-contracts/routingslip) should never be published).

```csharp
public abstract bool ConfigureConsumeTopology { set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **InstanceId**

Specifies an identifier that uniquely identifies the endpoint instance, which is appended to the
 end of the endpoint name.

```csharp
public abstract string InstanceId { set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **AddConfigureEndpointCallback(Action\<IReceiveEndpointConfigurator\>)**

Add an endpoint configuration callback to the registration

```csharp
void AddConfigureEndpointCallback(Action<IReceiveEndpointConfigurator> callback)
```

#### Parameters

`callback` [Action\<IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **AddConfigureEndpointCallback(Action\<IRegistrationContext, IReceiveEndpointConfigurator\>)**

Add an endpoint configuration callback to the registration

```csharp
void AddConfigureEndpointCallback(Action<IRegistrationContext, IReceiveEndpointConfigurator> callback)
```

#### Parameters

`callback` [Action\<IRegistrationContext, IReceiveEndpointConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
