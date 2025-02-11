---

title: ScheduleSettings<TInstance, TMessage>

---

# ScheduleSettings\<TInstance, TMessage\>

Namespace: MassTransit

The schedule settings, including the default delay for the message

```csharp
public interface ScheduleSettings<TInstance, TMessage>
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

## Properties

### **DelayProvider**

Provides the delay for the message

```csharp
public abstract ScheduleDelayProvider<TInstance> DelayProvider { get; }
```

#### Property Value

[ScheduleDelayProvider\<TInstance\>](../masstransit/scheduledelayprovider-1)<br/>

### **Received**

Configure the received correlation

```csharp
public abstract Action<IEventCorrelationConfigurator<TInstance, TMessage>> Received { get; }
```

#### Property Value

[Action\<IEventCorrelationConfigurator\<TInstance, TMessage\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
