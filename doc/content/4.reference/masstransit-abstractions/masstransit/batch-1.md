---

title: Batch<T>

---

# Batch\<T\>

Namespace: MassTransit

A batch of messages which are delivered to a consumer all at once

```csharp
public interface Batch<T> : IEnumerable<ConsumeContext<T>>, IEnumerable
```

#### Type Parameters

`T`<br/>

Implements [IEnumerable\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Mode**

```csharp
public abstract BatchCompletionMode Mode { get; }
```

#### Property Value

[BatchCompletionMode](../masstransit/batchcompletionmode)<br/>

### **FirstMessageReceived**

When the first message in this batch was received

```csharp
public abstract DateTime FirstMessageReceived { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **LastMessageReceived**

When the last message in this batch was received

```csharp
public abstract DateTime LastMessageReceived { get; }
```

#### Property Value

[DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>

### **Item**

```csharp
public abstract ConsumeContext<T> Item { get; }
```

#### Property Value

[ConsumeContext\<T\>](../masstransit/consumecontext-1)<br/>

### **Length**

The number of messages in this batch

```csharp
public abstract int Length { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
