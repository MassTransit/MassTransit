---

title: RoutingKeyConventionExtensions

---

# RoutingKeyConventionExtensions

Namespace: MassTransit

```csharp
public static class RoutingKeyConventionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingKeyConventionExtensions](../masstransit/routingkeyconventionextensions)

## Methods

### **UseRoutingKeyFormatter\<T\>(IMessageSendTopologyConfigurator\<T\>, IMessageRoutingKeyFormatter\<T\>)**

```csharp
public static void UseRoutingKeyFormatter<T>(IMessageSendTopologyConfigurator<T> configurator, IMessageRoutingKeyFormatter<T> formatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessageSendTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator-1)<br/>

`formatter` [IMessageRoutingKeyFormatter\<T\>](../masstransit-transports/imessageroutingkeyformatter-1)<br/>

### **UseRoutingKeyFormatter\<T\>(ISendTopologyConfigurator, IMessageRoutingKeyFormatter\<T\>)**

Use the routing key formatter for the specified message type

```csharp
public static void UseRoutingKeyFormatter<T>(ISendTopologyConfigurator configurator, IMessageRoutingKeyFormatter<T> formatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator)<br/>

`formatter` [IMessageRoutingKeyFormatter\<T\>](../masstransit-transports/imessageroutingkeyformatter-1)<br/>

### **UseRoutingKeyFormatter\<T\>(ISendTopologyConfigurator, Func\<SendContext\<T\>, String\>)**

Use the delegate to format the routing key

```csharp
public static void UseRoutingKeyFormatter<T>(ISendTopologyConfigurator configurator, Func<SendContext<T>, string> formatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISendTopologyConfigurator](../../masstransit-abstractions/masstransit/isendtopologyconfigurator)<br/>

`formatter` [Func\<SendContext\<T\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **UseRoutingKeyFormatter\<T\>(IMessageSendTopologyConfigurator\<T\>, Func\<SendContext\<T\>, String\>)**

Use the delegate to format the routing key

```csharp
public static void UseRoutingKeyFormatter<T>(IMessageSendTopologyConfigurator<T> configurator, Func<SendContext<T>, string> formatter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessageSendTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator-1)<br/>

`formatter` [Func\<SendContext\<T\>, String\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
