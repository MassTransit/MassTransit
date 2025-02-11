---

title: ReceiveEndpointLoggingExtensions

---

# ReceiveEndpointLoggingExtensions

Namespace: MassTransit.Transports

```csharp
public static class ReceiveEndpointLoggingExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceiveEndpointLoggingExtensions](../masstransit-transports/receiveendpointloggingextensions)

## Methods

### **LogSkipped(ReceiveContext)**

Log a skipped message that was moved to the dead-letter queue

```csharp
public static void LogSkipped(ReceiveContext context)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

### **LogMoved(ReceiveContext, String, String)**

Log a moved message from one endpoint to the destination endpoint address

```csharp
public static void LogMoved(ReceiveContext context, string destination, string reason)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`destination` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`reason` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LogConsumed\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

Log a consumed message

```csharp
public static void LogConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LogFaulted\<T\>(ConsumeContext\<T\>, TimeSpan, String, Exception)**

```csharp
public static void LogFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **LogCanceled\<T\>(ConsumeContext\<T\>, TimeSpan, String)**

```csharp
public static void LogCanceled<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`duration` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`consumerType` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LogFaulted(ReceiveContext, Exception)**

```csharp
public static void LogFaulted(ReceiveContext context, Exception exception)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **LogTransportDupe\<TTransportMessageId\>(ReceiveContext, TTransportMessageId)**

```csharp
public static void LogTransportDupe<TTransportMessageId>(ReceiveContext context, TTransportMessageId transportMessageId)
```

#### Type Parameters

`TTransportMessageId`<br/>

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`transportMessageId` TTransportMessageId<br/>

### **LogTransportFaulted(ReceiveContext, Exception)**

```csharp
public static void LogTransportFaulted(ReceiveContext context, Exception exception)
```

#### Parameters

`context` [ReceiveContext](../../masstransit-abstractions/masstransit/receivecontext)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **LogRetry(ConsumeContext, Exception)**

```csharp
public static void LogRetry(ConsumeContext context, Exception exception)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **LogRetry\<TContext\>(TContext, Exception)**

```csharp
public static void LogRetry<TContext>(TContext context, Exception exception)
```

#### Type Parameters

`TContext`<br/>

#### Parameters

`context` TContext<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **LogFaulted\<T\>(SendContext\<T\>, Exception)**

```csharp
public static void LogFaulted<T>(SendContext<T> context, Exception exception)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **LogSent\<T\>(SendContext\<T\>)**

```csharp
public static void LogSent<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

### **LogConsumerCompleted(ReceiveEndpointContext, Int64, Int32)**

```csharp
public static void LogConsumerCompleted(ReceiveEndpointContext context, long deliveryCount, int concurrentDeliveryCount)
```

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`deliveryCount` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`concurrentDeliveryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **LogConsumerCompleted(ReceiveEndpointContext, Int64, Int32, String)**

```csharp
public static void LogConsumerCompleted(ReceiveEndpointContext context, long deliveryCount, int concurrentDeliveryCount, string tag)
```

#### Parameters

`context` [ReceiveEndpointContext](../masstransit-transports/receiveendpointcontext)<br/>

`deliveryCount` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`concurrentDeliveryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`tag` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **LogScheduled\<T\>(SendContext\<T\>, DateTime)**

```csharp
public static void LogScheduled<T>(SendContext<T> context, DateTime deliveryTime)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

`deliveryTime` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
