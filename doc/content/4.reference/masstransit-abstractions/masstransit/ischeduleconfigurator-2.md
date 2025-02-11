---

title: IScheduleConfigurator<TInstance, TMessage>

---

# IScheduleConfigurator\<TInstance, TMessage\>

Namespace: MassTransit

```csharp
public interface IScheduleConfigurator<TInstance, TMessage>
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

## Properties

### **Delay**

Set a fixed message delay, which is applied to all scheduled messages unless
 overriden by the .Schedule method.

```csharp
public abstract TimeSpan Delay { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **DelayProvider**

Set a dynamic message delay provider, which uses the instance to determine the delay
 unless overriden by the .Schedule method.

```csharp
public abstract ScheduleDelayProvider<TInstance> DelayProvider { set; }
```

#### Property Value

[ScheduleDelayProvider\<TInstance\>](../masstransit/scheduledelayprovider-1)<br/>

### **Received**

Configure the behavior of the Received event, the same was Events are configured on
 the state machine.

```csharp
public abstract Action<IEventCorrelationConfigurator<TInstance, TMessage>> Received { set; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
