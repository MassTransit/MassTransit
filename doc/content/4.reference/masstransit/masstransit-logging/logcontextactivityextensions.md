---

title: LogContextActivityExtensions

---

# LogContextActivityExtensions

Namespace: MassTransit.Logging

```csharp
public static class LogContextActivityExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LogContextActivityExtensions](../masstransit-logging/logcontextactivityextensions)

## Methods

### **StartSendActivity\<T\>(ILogContext, SendTransportContext, SendContext\<T\>, ValueTuple`2[])**

```csharp
public static Nullable<StartedActivity> StartSendActivity<T>(ILogContext logContext, SendTransportContext transportContext, SendContext<T> context, ValueTuple`2[] tags)
```

#### Type Parameters

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`transportContext` [SendTransportContext](../masstransit-transports/sendtransportcontext)<br/>

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`tags` [ValueTuple`2[]](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple-2)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartOutboxSendActivity\<T\>(ILogContext, SendContext\<T\>)**

```csharp
public static Nullable<StartedActivity> StartOutboxSendActivity<T>(ILogContext logContext, SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartOutboxDeliverActivity(ILogContext, OutboxMessageContext)**

```csharp
public static Nullable<StartedActivity> StartOutboxDeliverActivity(ILogContext logContext, OutboxMessageContext context)
```

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [OutboxMessageContext](../masstransit-middleware/outboxmessagecontext)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartReceiveActivity(ILogContext, String, String, String, ReceiveContext)**

```csharp
public static Nullable<StartedActivity> StartReceiveActivity(ILogContext logContext, string name, string inputAddress, string endpointName, ReceiveContext context)
```

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`inputAddress` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`endpointName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartConsumerActivity\<TConsumer, T\>(ILogContext, ConsumeContext\<T\>)**

```csharp
public static Nullable<StartedActivity> StartConsumerActivity<TConsumer, T>(ILogContext logContext, ConsumeContext<T> context)
```

#### Type Parameters

`TConsumer`<br/>

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartHandlerActivity\<T\>(ILogContext, ConsumeContext\<T\>)**

```csharp
public static Nullable<StartedActivity> StartHandlerActivity<T>(ILogContext logContext, ConsumeContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartSagaActivity\<TSaga, T\>(ILogContext, SagaConsumeContext\<TSaga, T\>)**

```csharp
public static Nullable<StartedActivity> StartSagaActivity<TSaga, T>(ILogContext logContext, SagaConsumeContext<TSaga, T> context)
```

#### Type Parameters

`TSaga`<br/>

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [SagaConsumeContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartSagaStateMachineActivity\<TSaga, T\>(ILogContext, BehaviorContext\<TSaga, T\>)**

```csharp
public static Nullable<StartedActivity> StartSagaStateMachineActivity<TSaga, T>(ILogContext logContext, BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`TSaga`<br/>

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [BehaviorContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartExecuteActivity\<TActivity, TArguments\>(ILogContext, ConsumeContext\<RoutingSlip\>)**

```csharp
public static Nullable<StartedActivity> StartExecuteActivity<TActivity, TArguments>(ILogContext logContext, ConsumeContext<RoutingSlip> context)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartCompensateActivity\<TActivity, TLog\>(ILogContext, ConsumeContext\<RoutingSlip\>)**

```csharp
public static Nullable<StartedActivity> StartCompensateActivity<TActivity, TLog>(ILogContext logContext, ConsumeContext<RoutingSlip> context)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartGenericActivity(ILogContext, String)**

```csharp
public static Nullable<StartedActivity> StartGenericActivity(ILogContext logContext, string operationName)
```

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`operationName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[Nullable\<StartedActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
