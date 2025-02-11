---

title: ConsumerConnectorCache

---

# ConsumerConnectorCache

Namespace: MassTransit.Configuration

```csharp
public static class ConsumerConnectorCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerConnectorCache](../masstransit-configuration/consumerconnectorcache)

## Methods

### **Connect(IConsumePipeConnector, Type, Func\<Type, Object\>)**

```csharp
public static ConnectHandle Connect(IConsumePipeConnector consumePipe, Type consumerType, Func<Type, object> objectFactory)
```

#### Parameters

`consumePipe` [IConsumePipeConnector](../../masstransit-abstractions/masstransit/iconsumepipeconnector)<br/>

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`objectFactory` [Func\<Type, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
