---

title: InstanceExtensions

---

# InstanceExtensions

Namespace: MassTransit

Extensions for subscribing object instances.

```csharp
public static class InstanceExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InstanceExtensions](../masstransit/instanceextensions)

## Methods

### **Instance(IReceiveEndpointConfigurator, Object)**

Subscribes an object instance to the bus

```csharp
public static void Instance(IReceiveEndpointConfigurator configurator, object instance)
```

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
Service Bus Service Configurator
 - the item that is passed as a parameter to
 the action that is calling the configurator.

`instance` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>
The instance to subscribe.

### **ConnectInstance(IConsumePipeConnector, Object)**

Connects any consumers for the object to the message dispatcher

```csharp
public static ConnectHandle ConnectInstance(IConsumePipeConnector connector, object instance)
```

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>
The service bus to configure

`instance` [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
The unsubscribe action that can be called to unsubscribe the instance
 passed as an argument.

### **Instance\<T\>(IReceiveEndpointConfigurator, T, Action\<IInstanceConfigurator\<T\>\>)**

Subscribes an object instance to the bus

```csharp
public static void Instance<T>(IReceiveEndpointConfigurator configurator, T instance, Action<IInstanceConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>
Service Bus Service Configurator
 - the item that is passed as a parameter to
 the action that is calling the configurator.

`instance` T<br/>
The instance to subscribe.

`configure` [Action\<IInstanceConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Configure the instance

### **ConnectInstance\<T\>(IConsumePipeConnector, T)**

Connects any consumers for the object to the message dispatcher

```csharp
public static ConnectHandle ConnectInstance<T>(IConsumePipeConnector connector, T instance)
```

#### Type Parameters

`T`<br/>
The consumer type

#### Parameters

`connector` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>
The service bus instance to call this method on.

`instance` T<br/>
The instance to subscribe.

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
The unsubscribe action that can be called to unsubscribe the instance
 passed as an argument.
