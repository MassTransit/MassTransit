---

title: Retry

---

# Retry

Namespace: MassTransit

```csharp
public static class Retry
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Retry](../masstransit/retry)

## Properties

### **None**

Create a policy that does not retry any messages

```csharp
public static IRetryPolicy None { get; }
```

#### Property Value

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

## Methods

### **Immediate(Int32)**

Create an immediate retry policy with the specified number of retries, with no
 delay between attempts.

```csharp
public static IRetryPolicy Immediate(int retryLimit)
```

#### Parameters

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retries to attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Immediate(IExceptionFilter, Int32)**

Create an immediate retry policy with the specified number of retries, with no
 delay between attempts.

```csharp
public static IRetryPolicy Immediate(IExceptionFilter filter, int retryLimit)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retries to attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Intervals(TimeSpan[])**

Create an interval retry policy with the specified intervals. The retry count equals
 the number of intervals provided

```csharp
public static IRetryPolicy Intervals(TimeSpan[] intervals)
```

#### Parameters

`intervals` [TimeSpan[]](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The intervals before each subsequent retry attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Intervals(IExceptionFilter, TimeSpan[])**

Create an interval retry policy with the specified intervals. The retry count equals
 the number of intervals provided

```csharp
public static IRetryPolicy Intervals(IExceptionFilter filter, TimeSpan[] intervals)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`intervals` [TimeSpan[]](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The intervals before each subsequent retry attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Intervals(Int32[])**

Create an interval retry policy with the specified intervals. The retry count equals
 the number of intervals provided

```csharp
public static IRetryPolicy Intervals(Int32[] intervals)
```

#### Parameters

`intervals` [Int32[]](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The intervals before each subsequent retry attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Intervals(IExceptionFilter, Int32[])**

Create an interval retry policy with the specified intervals. The retry count equals
 the number of intervals provided

```csharp
public static IRetryPolicy Intervals(IExceptionFilter filter, Int32[] intervals)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`intervals` [Int32[]](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The intervals before each subsequent retry attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Interval(Int32, TimeSpan)**

Create an interval retry policy with the specified number of retries at a fixed interval

```csharp
public static IRetryPolicy Interval(int retryCount, TimeSpan interval)
```

#### Parameters

`retryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retry attempts

`interval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The interval between each retry attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Interval(Int32, Int32)**

Create an interval retry policy with the specified number of retries at a fixed interval

```csharp
public static IRetryPolicy Interval(int retryCount, int interval)
```

#### Parameters

`retryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retry attempts

`interval` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The interval between each retry attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Interval(IExceptionFilter, Int32, TimeSpan)**

Create an interval retry policy with the specified number of retries at a fixed interval

```csharp
public static IRetryPolicy Interval(IExceptionFilter filter, int retryCount, TimeSpan interval)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`retryCount` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retry attempts

`interval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The interval between each retry attempt

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Exponential(Int32, TimeSpan, TimeSpan, TimeSpan)**

Create an exponential retry policy with the specified number of retries at exponential
 intervals

```csharp
public static IRetryPolicy Exponential(int retryLimit, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan intervalDelta)
```

#### Parameters

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`maxInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`intervalDelta` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Exponential(TimeSpan, TimeSpan, TimeSpan)**

Create an exponential retry policy that never gives up
 intervals

```csharp
public static IRetryPolicy Exponential(TimeSpan minInterval, TimeSpan maxInterval, TimeSpan intervalDelta)
```

#### Parameters

`minInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`maxInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`intervalDelta` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Exponential(IExceptionFilter, Int32, TimeSpan, TimeSpan, TimeSpan)**

Create an exponential retry policy with the specified number of retries at exponential
 intervals

```csharp
public static IRetryPolicy Exponential(IExceptionFilter filter, int retryLimit, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan intervalDelta)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`minInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`maxInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

`intervalDelta` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Incremental(Int32, TimeSpan, TimeSpan)**

Create an incremental retry policy with the specified number of retry attempts with an incrementing
 interval between retries

```csharp
public static IRetryPolicy Incremental(int retryLimit, TimeSpan initialInterval, TimeSpan intervalIncrement)
```

#### Parameters

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retry attempts

`initialInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The initial retry interval

`intervalIncrement` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The interval to add to the retry interval with each subsequent retry

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Incremental(IExceptionFilter, Int32, TimeSpan, TimeSpan)**

Create an incremental retry policy with the specified number of retry attempts with an incrementing
 interval between retries

```csharp
public static IRetryPolicy Incremental(IExceptionFilter filter, int retryLimit, TimeSpan initialInterval, TimeSpan intervalIncrement)
```

#### Parameters

`filter` [IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

`retryLimit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The number of retry attempts

`initialInterval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The initial retry interval

`intervalIncrement` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The interval to add to the retry interval with each subsequent retry

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **CreatePolicy(Action\<IRetryConfigurator\>)**

```csharp
public static IRetryPolicy CreatePolicy(Action<IRetryConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRetryConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IRetryPolicy](../../masstransit-abstractions/masstransit/iretrypolicy)<br/>

### **Except(Type[])**

Retry all exceptions except for the exception types specified

```csharp
public static IExceptionFilter Except(Type[] exceptionTypes)
```

#### Parameters

`exceptionTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **Except\<T1\>()**

Retry all exceptions except for the exception types specified

```csharp
public static IExceptionFilter Except<T1>()
```

#### Type Parameters

`T1`<br/>

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **Except\<T1, T2\>()**

Retry all exceptions except for the exception types specified

```csharp
public static IExceptionFilter Except<T1, T2>()
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **Except\<T1, T2, T3\>()**

Retry all exceptions except for the exception types specified

```csharp
public static IExceptionFilter Except<T1, T2, T3>()
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **Selected(Type[])**

Retry only the exception types specified

```csharp
public static IExceptionFilter Selected(Type[] exceptionTypes)
```

#### Parameters

`exceptionTypes` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **Selected\<T1\>()**

Retry only the exception types specified

```csharp
public static IExceptionFilter Selected<T1>()
```

#### Type Parameters

`T1`<br/>

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **Selected\<T1, T2\>()**

Retry only the exception types specified

```csharp
public static IExceptionFilter Selected<T1, T2>()
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **Selected\<T1, T2, T3\>()**

Retry only the exception types specified

```csharp
public static IExceptionFilter Selected<T1, T2, T3>()
```

#### Type Parameters

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **All()**

Retry all exceptions

```csharp
public static IExceptionFilter All()
```

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

### **Filter\<T\>(Func\<T, Boolean\>)**

Filter an exception type

```csharp
public static IExceptionFilter Filter<T>(Func<T, bool> filter)
```

#### Type Parameters

`T`<br/>
The exception type

#### Parameters

`filter` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The filter expression

#### Returns

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>
True if the exception should be retried, otherwise false
