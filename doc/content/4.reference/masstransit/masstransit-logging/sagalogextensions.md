---

title: SagaLogExtensions

---

# SagaLogExtensions

Namespace: MassTransit.Logging

```csharp
public static class SagaLogExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaLogExtensions](../masstransit-logging/sagalogextensions)

## Methods

### **LogUsed\<TSaga, TMessage\>(SagaConsumeContext\<TSaga, TMessage\>, Nullable\<Guid\>)**

```csharp
public static void LogUsed<TSaga, TMessage>(SagaConsumeContext<TSaga, TMessage> context, Nullable<Guid> correlationId)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`context` [SagaConsumeContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LogAdded\<TSaga, TMessage\>(SagaConsumeContext\<TSaga, TMessage\>, Nullable\<Guid\>)**

```csharp
public static void LogAdded<TSaga, TMessage>(SagaConsumeContext<TSaga, TMessage> context, Nullable<Guid> correlationId)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`context` [SagaConsumeContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LogCreated\<TSaga, TMessage\>(SagaConsumeContext\<TSaga, TMessage\>, Nullable\<Guid\>)**

```csharp
public static void LogCreated<TSaga, TMessage>(SagaConsumeContext<TSaga, TMessage> context, Nullable<Guid> correlationId)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`context` [SagaConsumeContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LogInsert\<TSaga, TMessage\>(ConsumeContext\<TMessage\>, Nullable\<Guid\>)**

```csharp
public static void LogInsert<TSaga, TMessage>(ConsumeContext<TMessage> context, Nullable<Guid> correlationId)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LogInsertFault\<TSaga, TMessage\>(ConsumeContext\<TMessage\>, Exception, Nullable\<Guid\>)**

```csharp
public static void LogInsertFault<TSaga, TMessage>(ConsumeContext<TMessage> context, Exception exception, Nullable<Guid> correlationId)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LogRemoved\<TSaga, TMessage\>(SagaConsumeContext\<TSaga, TMessage\>, Nullable\<Guid\>)**

```csharp
public static void LogRemoved<TSaga, TMessage>(SagaConsumeContext<TSaga, TMessage> context, Nullable<Guid> correlationId)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`context` [SagaConsumeContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/sagaconsumecontext-2)<br/>

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

### **LogFault\<TSaga, TMessage\>(ConsumeContext\<TMessage\>, Exception, Nullable\<Guid\>)**

```csharp
public static void LogFault<TSaga, TMessage>(ConsumeContext<TMessage> context, Exception exception, Nullable<Guid> correlationId)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`context` [ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
