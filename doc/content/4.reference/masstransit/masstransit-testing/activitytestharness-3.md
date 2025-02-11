---

title: ActivityTestHarness<TActivity, TArguments, TLog>

---

# ActivityTestHarness\<TActivity, TArguments, TLog\>

Namespace: MassTransit.Testing

```csharp
public class ActivityTestHarness<TActivity, TArguments, TLog>
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActivityTestHarness\<TActivity, TArguments, TLog\>](../masstransit-testing/activitytestharness-3)

## Properties

### **ExecuteQueueName**

```csharp
public string ExecuteQueueName { get; private set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CompensateQueueName**

```csharp
public string CompensateQueueName { get; private set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CompensateAddress**

```csharp
public Uri CompensateAddress { get; private set; }
```

#### Property Value

Uri<br/>

### **Name**

```csharp
public string Name { get; private set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExecuteAddress**

```csharp
public Uri ExecuteAddress { get; private set; }
```

#### Property Value

Uri<br/>

## Constructors

### **ActivityTestHarness(BusTestHarness, IActivityFactory\<TActivity, TArguments, TLog\>, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

```csharp
public ActivityTestHarness(BusTestHarness testHarness, IActivityFactory<TActivity, TArguments, TLog> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute, Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`activityFactory` [IActivityFactory\<TActivity, TArguments, TLog\>](../../masstransit-abstractions/masstransit/iactivityfactory-3)<br/>

`configureExecute` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`configureCompensate` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Events

### **OnConfigureExecuteReceiveEndpoint**

```csharp
public event Action<IReceiveEndpointConfigurator> OnConfigureExecuteReceiveEndpoint;
```

### **OnConfigureCompensateReceiveEndpoint**

```csharp
public event Action<IReceiveEndpointConfigurator> OnConfigureCompensateReceiveEndpoint;
```
