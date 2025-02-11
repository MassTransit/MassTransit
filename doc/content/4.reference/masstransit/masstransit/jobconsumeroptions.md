---

title: JobConsumerOptions

---

# JobConsumerOptions

Namespace: MassTransit

```csharp
public class JobConsumerOptions : IOptions, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobConsumerOptions](../masstransit/jobconsumeroptions)<br/>
Implements [IOptions](../../masstransit-abstractions/masstransit-configuration/ioptions), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **HeartbeatInterval**

```csharp
public TimeSpan HeartbeatInterval { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

## Constructors

### **JobConsumerOptions()**

```csharp
public JobConsumerOptions()
```

## Methods

### **SetHeartbeatInterval(Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>)**

```csharp
public JobConsumerOptions SetHeartbeatInterval(Nullable<int> d, Nullable<int> h, Nullable<int> m, Nullable<int> s, Nullable<int> ms)
```

#### Parameters

`d` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`h` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`m` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`s` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`ms` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[JobConsumerOptions](../masstransit/jobconsumeroptions)<br/>

### **SetHeartbeatInterval(TimeSpan)**

```csharp
public JobConsumerOptions SetHeartbeatInterval(TimeSpan interval)
```

#### Parameters

`interval` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[JobConsumerOptions](../masstransit/jobconsumeroptions)<br/>
