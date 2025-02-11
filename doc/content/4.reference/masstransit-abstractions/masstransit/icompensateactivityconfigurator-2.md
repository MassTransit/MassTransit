---

title: ICompensateActivityConfigurator<TActivity, TLog>

---

# ICompensateActivityConfigurator\<TActivity, TLog\>

Namespace: MassTransit

Configure the execution of the activity and arguments with some tasty middleware.

```csharp
public interface ICompensateActivityConfigurator<TActivity, TLog> : IPipeConfigurator<CompensateActivityContext<TActivity, TLog>>, IActivityObserverConnector, IConsumeConfigurator
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

Implements [IPipeConfigurator\<CompensateActivityContext\<TActivity, TLog\>\>](../masstransit/ipipeconfigurator-1), [IActivityObserverConnector](../masstransit/iactivityobserverconnector), [IConsumeConfigurator](../masstransit/iconsumeconfigurator)

## Properties

### **ConcurrentMessageLimit**

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **Log(Action\<ICompensateLogConfigurator\<TLog\>\>)**

```csharp
void Log(Action<ICompensateLogConfigurator<TLog>> configure)
```

#### Parameters

`configure` [Action\<ICompensateLogConfigurator\<TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ActivityLog(Action\<ICompensateActivityLogConfigurator\<TLog\>\>)**

Configure the arguments separate from the activity

```csharp
void ActivityLog(Action<ICompensateActivityLogConfigurator<TLog>> configure)
```

#### Parameters

`configure` [Action\<ICompensateActivityLogConfigurator\<TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **RoutingSlip(Action\<IRoutingSlipConfigurator\>)**

Configure the routing slip pipe

```csharp
void RoutingSlip(Action<IRoutingSlipConfigurator> configure)
```

#### Parameters

`configure` [Action\<IRoutingSlipConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
