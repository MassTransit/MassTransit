---

title: StateMachineScheduleConfigurator<TInstance, TMessage>

---

# StateMachineScheduleConfigurator\<TInstance, TMessage\>

Namespace: MassTransit.Configuration

```csharp
public class StateMachineScheduleConfigurator<TInstance, TMessage> : IScheduleConfigurator<TInstance, TMessage>, ScheduleSettings<TInstance, TMessage>
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StateMachineScheduleConfigurator\<TInstance, TMessage\>](../masstransit-configuration/statemachinescheduleconfigurator-2)<br/>
Implements [IScheduleConfigurator\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/ischeduleconfigurator-2), [ScheduleSettings\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedulesettings-2)

## Properties

### **Settings**

```csharp
public ScheduleSettings<TInstance, TMessage> Settings { get; }
```

#### Property Value

[ScheduleSettings\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/schedulesettings-2)<br/>

### **Delay**

```csharp
public TimeSpan Delay { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **DelayProvider**

```csharp
public ScheduleDelayProvider<TInstance> DelayProvider { get; set; }
```

#### Property Value

[ScheduleDelayProvider\<TInstance\>](../../masstransit-abstractions/masstransit/scheduledelayprovider-1)<br/>

## Constructors

### **StateMachineScheduleConfigurator()**

```csharp
public StateMachineScheduleConfigurator()
```
