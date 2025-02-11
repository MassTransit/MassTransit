---

title: LogContextInstrumentationExtensions

---

# LogContextInstrumentationExtensions

Namespace: MassTransit.Logging

```csharp
public static class LogContextInstrumentationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LogContextInstrumentationExtensions](../masstransit-logging/logcontextinstrumentationextensions)

## Methods

### **StartReceiveInstrument(ILogContext, ReceiveContext)**

```csharp
public static Nullable<StartedInstrument> StartReceiveInstrument(ILogContext logContext, ReceiveContext context)
```

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartHandlerInstrument\<TMessage\>(ILogContext, ConsumeContext\<TMessage\>, Stopwatch)**

```csharp
public static Nullable<StartedInstrument> StartHandlerInstrument<TMessage>(ILogContext logContext, ConsumeContext<TMessage> context, Stopwatch stopwatch)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`stopwatch` [Stopwatch](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartSagaInstrument\<TSaga, T\>(ILogContext, SagaConsumeContext\<TSaga, T\>)**

```csharp
public static Nullable<StartedInstrument> StartSagaInstrument<TSaga, T>(ILogContext logContext, SagaConsumeContext<TSaga, T> context)
```

#### Type Parameters

`TSaga`<br/>

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [SagaConsumeContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartSagaStateMachineInstrument\<TSaga, T\>(ILogContext, BehaviorContext\<TSaga, T\>)**

```csharp
public static Nullable<StartedInstrument> StartSagaStateMachineInstrument<TSaga, T>(ILogContext logContext, BehaviorContext<TSaga, T> context)
```

#### Type Parameters

`TSaga`<br/>

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [BehaviorContext\<TSaga, T\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartConsumeInstrument\<TConsumer, T\>(ILogContext, ConsumeContext\<T\>, Stopwatch)**

```csharp
public static Nullable<StartedInstrument> StartConsumeInstrument<TConsumer, T>(ILogContext logContext, ConsumeContext<T> context, Stopwatch timer)
```

#### Type Parameters

`TConsumer`<br/>

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`timer` [Stopwatch](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartActivityExecuteInstrument\<TActivity, TArguments\>(ILogContext, ConsumeContext\<RoutingSlip\>, Stopwatch)**

```csharp
public static Nullable<StartedInstrument> StartActivityExecuteInstrument<TActivity, TArguments>(ILogContext logContext, ConsumeContext<RoutingSlip> context, Stopwatch timer)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`timer` [Stopwatch](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartActivityCompensateInstrument\<TActivity, TLog\>(ILogContext, ConsumeContext\<RoutingSlip\>, Stopwatch)**

```csharp
public static Nullable<StartedInstrument> StartActivityCompensateInstrument<TActivity, TLog>(ILogContext logContext, ConsumeContext<RoutingSlip> context, Stopwatch timer)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [ConsumeContext\<RoutingSlip\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`timer` [Stopwatch](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartSendInstrument\<T\>(ILogContext, SendTransportContext, SendContext\<T\>)**

```csharp
public static Nullable<StartedInstrument> StartSendInstrument<T>(ILogContext logContext, SendTransportContext transportContext, SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`transportContext` [SendTransportContext](../masstransit-transports/sendtransportcontext)<br/>

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartOutboxSendInstrument\<T\>(ILogContext, SendContext\<T\>)**

```csharp
public static Nullable<StartedInstrument> StartOutboxSendInstrument<T>(ILogContext logContext, SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartOutboxDeliveryInstrument(ILogContext, OutboxMessageContext)**

```csharp
public static Nullable<StartedInstrument> StartOutboxDeliveryInstrument(ILogContext logContext, OutboxMessageContext context)
```

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`context` [OutboxMessageContext](../masstransit-middleware/outboxmessagecontext)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **StartOutboxDeliveryInstrument(ILogContext, OutboxConsumeContext, OutboxMessageContext)**

```csharp
public static Nullable<StartedInstrument> StartOutboxDeliveryInstrument(ILogContext logContext, OutboxConsumeContext consumeContext, OutboxMessageContext context)
```

#### Parameters

`logContext` [ILogContext](../masstransit-logging/ilogcontext)<br/>

`consumeContext` [OutboxConsumeContext](../masstransit-middleware/outboxconsumecontext)<br/>

`context` [OutboxMessageContext](../masstransit-middleware/outboxmessagecontext)<br/>

#### Returns

[Nullable\<StartedInstrument\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **TryConfigure(IServiceProvider)**

```csharp
public static void TryConfigure(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

### **TryConfigure(InstrumentationOptions)**

```csharp
public static void TryConfigure(InstrumentationOptions options)
```

#### Parameters

`options` [InstrumentationOptions](../masstransit-monitoring/instrumentationoptions)<br/>
