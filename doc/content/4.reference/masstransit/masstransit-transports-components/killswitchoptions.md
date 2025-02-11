---

title: KillSwitchOptions

---

# KillSwitchOptions

Namespace: MassTransit.Transports.Components

```csharp
public class KillSwitchOptions : IOptions, ISpecification
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>
Implements [IOptions](../../masstransit-abstractions/masstransit-configuration/ioptions), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **TrackingPeriod**

The time window for tracking exceptions

```csharp
public TimeSpan TrackingPeriod { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **RestartTimeout**

The wait time before restarting the receive endpoint

```csharp
public TimeSpan RestartTimeout { get; set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **TripThreshold**

The percentage of failed messages that triggers the kill switch. Should be 0-100, but seriously like 5-10.

```csharp
public int TripThreshold { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ActivationThreshold**

The number of messages that must be consumed before the kill switch activates.

```csharp
public int ActivationThreshold { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ExceptionFilter**

By default, all exceptions are tracked. An exception filter can be specified (using [KillSwitchOptions.SetExceptionFilter(Action\<IExceptionConfigurator\>)](killswitchoptions#setexceptionfilteractioniexceptionconfigurator) to only track
 specific exceptions.

```csharp
public IExceptionFilter ExceptionFilter { get; private set; }
```

#### Property Value

[IExceptionFilter](../../masstransit-abstractions/masstransit/iexceptionfilter)<br/>

## Constructors

### **KillSwitchOptions()**

```csharp
public KillSwitchOptions()
```

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **SetActivationThreshold(Int32)**

The number of messages that must be consumed before the kill switch activates.

```csharp
public KillSwitchOptions SetActivationThreshold(int value)
```

#### Parameters

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>

### **SetTripThreshold(Int32)**

The percentage of failed messages that triggers the kill switch. Should be 0-100, but seriously like 5-10.

```csharp
public KillSwitchOptions SetTripThreshold(int value)
```

#### Parameters

`value` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>

### **SetTripThreshold(Double)**

The percentage of failed messages that triggers the kill switch. Should be 0-100, but seriously like 5-10.

```csharp
public KillSwitchOptions SetTripThreshold(double percentage)
```

#### Parameters

`percentage` [Double](https://learn.microsoft.com/en-us/dotnet/api/system.double)<br/>

#### Returns

[KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>

### **SetTrackingPeriod(TimeSpan)**

The time window for tracking exceptions

```csharp
public KillSwitchOptions SetTrackingPeriod(TimeSpan value)
```

#### Parameters

`value` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>

### **SetTrackingPeriod(Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>)**

The time window for tracking exceptions

```csharp
public KillSwitchOptions SetTrackingPeriod(Nullable<int> d, Nullable<int> h, Nullable<int> m, Nullable<int> s, Nullable<int> ms)
```

#### Parameters

`d` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`h` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`m` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`s` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`ms` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>

### **SetRestartTimeout(TimeSpan)**

The wait time before restarting the receive endpoint

```csharp
public KillSwitchOptions SetRestartTimeout(TimeSpan value)
```

#### Parameters

`value` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

#### Returns

[KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>

### **SetRestartTimeout(Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>, Nullable\<Int32\>)**

The wait time before restarting the receive endpoint

```csharp
public KillSwitchOptions SetRestartTimeout(Nullable<int> d, Nullable<int> h, Nullable<int> m, Nullable<int> s, Nullable<int> ms)
```

#### Parameters

`d` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`h` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`m` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`s` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`ms` [Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

#### Returns

[KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>

### **SetExceptionFilter(Action\<IExceptionConfigurator\>)**

By default, all exceptions are tracked. An exception filter can be configured to only track specific exceptions.

```csharp
public KillSwitchOptions SetExceptionFilter(Action<IExceptionConfigurator> configure)
```

#### Parameters

`configure` [Action\<IExceptionConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[KillSwitchOptions](../masstransit-transports-components/killswitchoptions)<br/>
