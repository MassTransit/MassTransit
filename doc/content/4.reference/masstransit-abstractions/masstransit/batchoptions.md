---

title: BatchOptions

---

# BatchOptions

Namespace: MassTransit

Batch options are applied to a [Batch\<T\>](../masstransit/batch-1) consumer to configure
 the size and time limits for each batch.

```csharp
public class BatchOptions : IOptions, IConfigureReceiveEndpoint, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchOptions](../masstransit/batchoptions)<br/>
Implements [IOptions](../masstransit-configuration/ioptions), [IConfigureReceiveEndpoint](../masstransit/iconfigurereceiveendpoint), [ISpecification](../masstransit/ispecification)

## Properties

### **MessageLimit**

The maximum number of messages in a single batch

```csharp
public int MessageLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrencyLimit**

The number of batches which can be executed concurrently

```csharp
public int ConcurrencyLimit { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **TimeLimit**

The maximum time to wait before delivering a partial batch

```csharp
public TimeSpan TimeLimit { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TimeLimitStart**

The starting point for the [BatchOptions.TimeLimit](batchoptions#timelimit)

```csharp
public BatchTimeLimitStart TimeLimitStart { get; set; }
```

#### Property Value

[BatchTimeLimitStart](../masstransit/batchtimelimitstart)<br/>

### **GroupKeyProvider**

The property to group by

```csharp
public object GroupKeyProvider { get; private set; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **BatchOptions()**

```csharp
public BatchOptions()
```

## Methods

### **Configure(String, IReceiveEndpointConfigurator)**

```csharp
public void Configure(string name, IReceiveEndpointConfigurator configurator)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`configurator` [IReceiveEndpointConfigurator](../masstransit/ireceiveendpointconfigurator)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SetConfigurationCallback(ConfigurationCallback)**

```csharp
public BatchOptions SetConfigurationCallback(ConfigurationCallback callback)
```

#### Parameters

`callback` [ConfigurationCallback](../masstransit/configurationcallback)<br/>

#### Returns

[BatchOptions](../masstransit/batchoptions)<br/>

### **SetMessageLimit(Int32)**

Sets the maximum number of messages in a single batch

```csharp
public BatchOptions SetMessageLimit(int limit)
```

#### Parameters

`limit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The message limit

#### Returns

[BatchOptions](../masstransit/batchoptions)<br/>

### **SetConcurrencyLimit(Int32)**

Sets the number of batches which can be executed concurrently

```csharp
public BatchOptions SetConcurrencyLimit(int limit)
```

#### Parameters

`limit` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
The message limit

#### Returns

[BatchOptions](../masstransit/batchoptions)<br/>

### **SetTimeLimit(TimeSpan)**

Sets the maximum time to wait before delivering a partial batch

```csharp
public BatchOptions SetTimeLimit(TimeSpan limit)
```

#### Parameters

`limit` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
The message limit

#### Returns

[BatchOptions](../masstransit/batchoptions)<br/>

### **SetTimeLimitStart(BatchTimeLimitStart)**

Sets the starting point for the [BatchOptions.TimeLimit](batchoptions#timelimit)

```csharp
public BatchOptions SetTimeLimitStart(BatchTimeLimitStart timeLimitStart)
```

#### Parameters

`timeLimitStart` [BatchTimeLimitStart](../masstransit/batchtimelimitstart)<br/>
The starting point

#### Returns

[BatchOptions](../masstransit/batchoptions)<br/>

### **SetTimeLimit(Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>)**

Sets the maximum time to wait before delivering a partial batch

```csharp
public BatchOptions SetTimeLimit(Nullable<int> ms, Nullable<int> s, Nullable<int> m, Nullable<int> h, Nullable<int> d)
```

#### Parameters

`ms` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`s` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`m` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`h` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`d` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[BatchOptions](../masstransit/batchoptions)<br/>

### **GroupBy\<T, TProperty\>(Func\<ConsumeContext\<T\>, Nullable\<TProperty\>\>)**

```csharp
public BatchOptions GroupBy<T, TProperty>(Func<ConsumeContext<T>, Nullable<TProperty>> provider)
```

#### Type Parameters

`T`<br/>

`TProperty`<br/>

#### Parameters

`provider` [Func\<ConsumeContext\<T\>, Nullable\<TProperty\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[BatchOptions](../masstransit/batchoptions)<br/>

### **GroupBy\<T, TProperty\>(Func\<ConsumeContext\<T\>, TProperty\>)**

```csharp
public BatchOptions GroupBy<T, TProperty>(Func<ConsumeContext<T>, TProperty> provider)
```

#### Type Parameters

`T`<br/>

`TProperty`<br/>

#### Parameters

`provider` [Func\<ConsumeContext\<T\>, TProperty\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[BatchOptions](../masstransit/batchoptions)<br/>
