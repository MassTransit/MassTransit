---

title: IBusInstance

---

# IBusInstance

Namespace: MassTransit.Transports

```csharp
public interface IBusInstance : IReceiveEndpointConnector
```

Implements [IReceiveEndpointConnector](../masstransit/ireceiveendpointconnector)

## Properties

### **Name**

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **InstanceType**

```csharp
public abstract Type InstanceType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **Bus**

```csharp
public abstract IBus Bus { get; }
```

#### Property Value

[IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

### **BusControl**

```csharp
public abstract IBusControl BusControl { get; }
```

#### Property Value

[IBusControl](../../masstransit-abstractions/masstransit/ibuscontrol)<br/>

### **HostConfiguration**

```csharp
public abstract IHostConfiguration HostConfiguration { get; }
```

#### Property Value

[IHostConfiguration](../masstransit-configuration/ihostconfiguration)<br/>

## Methods

### **Connect\<TRider\>(IRiderControl)**

```csharp
void Connect<TRider>(IRiderControl riderControl)
```

#### Type Parameters

`TRider`<br/>

#### Parameters

`riderControl` [IRiderControl](../../masstransit-abstractions/masstransit-transports/iridercontrol)<br/>

### **GetRider\<TRider\>()**

```csharp
TRider GetRider<TRider>()
```

#### Type Parameters

`TRider`<br/>

#### Returns

TRider<br/>
