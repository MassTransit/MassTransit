---

title: CorrelationIdConventionExtensions

---

# CorrelationIdConventionExtensions

Namespace: MassTransit

```csharp
public static class CorrelationIdConventionExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CorrelationIdConventionExtensions](../masstransit/correlationidconventionextensions)

## Methods

### **UseCorrelationId\<T\>(IMessageSendTopologyConfigurator\<T\>, Func\<T, Guid\>)**

Specify for the message type that the delegate be used for setting the CorrelationId
 property of the message envelope.

```csharp
public static void UseCorrelationId<T>(IMessageSendTopologyConfigurator<T> configurator, Func<T, Guid> correlationIdSelector)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessageSendTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator-1)<br/>

`correlationIdSelector` [Func\<T, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **UseCorrelationId\<T\>(IMessageSendTopologyConfigurator\<T\>, Func\<T, Nullable\<Guid\>\>)**

Specify for the message type that the delegate be used for setting the CorrelationId
 property of the message envelope.

```csharp
public static void UseCorrelationId<T>(IMessageSendTopologyConfigurator<T> configurator, Func<T, Nullable<Guid>> correlationIdSelector)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessageSendTopologyConfigurator\<T\>](../../masstransit-abstractions/masstransit/imessagesendtopologyconfigurator-1)<br/>

`correlationIdSelector` [Func\<T, Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **UseCorrelationId\<T\>(ISendTopology, Func\<T, Guid\>)**

Specify for the message type that the delegate be used for setting the CorrelationId
 property of the message envelope.

```csharp
public static void UseCorrelationId<T>(ISendTopology configurator, Func<T, Guid> correlationIdSelector)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISendTopology](../../masstransit-abstractions/masstransit/isendtopology)<br/>

`correlationIdSelector` [Func\<T, Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **UseCorrelationId\<T\>(ISendTopology, Func\<T, Nullable\<Guid\>\>)**

Specify for the message type that the delegate be used for setting the CorrelationId
 property of the message envelope.

```csharp
public static void UseCorrelationId<T>(ISendTopology configurator, Func<T, Nullable<Guid>> correlationIdSelector)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISendTopology](../../masstransit-abstractions/masstransit/isendtopology)<br/>

`correlationIdSelector` [Func\<T, Nullable\<Guid\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
