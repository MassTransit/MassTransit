---

title: PartitionKeyConventionExtensions

---

# PartitionKeyConventionExtensions

Namespace: MassTransit

```csharp
public static class PartitionKeyConventionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PartitionKeyConventionExtensions](../masstransit/partitionkeyconventionextensions)

## Methods

### **UsePartitionKeyFormatter\<T\>(IMessageSendTopologyConfigurator\<T\>, IMessagePartitionKeyFormatter\<T\>)**

```csharp
public static void UsePartitionKeyFormatter<T>(IMessageSendTopologyConfigurator<T> configurator, IMessagePartitionKeyFormatter<T> formatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessageSendTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator-1)<br/>

`formatter` [IMessagePartitionKeyFormatter\<T\>](../masstransit-transports/imessagepartitionkeyformatter-1)<br/>

### **UsePartitionKeyFormatter\<T\>(ISendTopologyConfigurator, IMessagePartitionKeyFormatter\<T\>)**

Use the partition key formatter for the specified message type

```csharp
public static void UsePartitionKeyFormatter<T>(ISendTopologyConfigurator configurator, IMessagePartitionKeyFormatter<T> formatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator)<br/>

`formatter` [IMessagePartitionKeyFormatter\<T\>](../masstransit-transports/imessagepartitionkeyformatter-1)<br/>

### **UsePartitionKeyFormatter\<T\>(ISendTopologyConfigurator, Func\<SendContext\<T\>, String\>)**

Use the delegate to format the partition key, using Empty if the string is null upon return

```csharp
public static void UsePartitionKeyFormatter<T>(ISendTopologyConfigurator configurator, Func<SendContext<T>, string> formatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator)<br/>

`formatter` [Func\<SendContext\<T\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **UsePartitionKeyFormatter\<T\>(IMessageSendTopologyConfigurator\<T\>, Func\<SendContext\<T\>, String\>)**

Use the delegate to format the partition key, using Empty if the string is null upon return

```csharp
public static void UsePartitionKeyFormatter<T>(IMessageSendTopologyConfigurator<T> configurator, Func<SendContext<T>, string> formatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessageSendTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator-1)<br/>

`formatter` [Func\<SendContext\<T\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
