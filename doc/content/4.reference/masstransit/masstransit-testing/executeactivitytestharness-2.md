---

title: ExecuteActivityTestHarness<TActivity, TArguments>

---

# ExecuteActivityTestHarness\<TActivity, TArguments\>

Namespace: MassTransit.Testing

```csharp
public class ExecuteActivityTestHarness<TActivity, TArguments>
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExecuteActivityTestHarness\<TActivity, TArguments\>](../masstransit-testing/executeactivitytestharness-2)

## Properties

### **ExecuteQueueName**

```csharp
public string ExecuteQueueName { get; private set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

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

### **ExecuteActivityTestHarness(BusTestHarness, IExecuteActivityFactory\<TActivity, TArguments\>, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public ExecuteActivityTestHarness(BusTestHarness testHarness, IExecuteActivityFactory<TActivity, TArguments> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute)
```

#### Parameters

`testHarness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`activityFactory` [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2)<br/>

`configureExecute` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

## Events

### **OnConfigureExecuteReceiveEndpoint**

```csharp
public event Action<IReceiveEndpointConfigurator> OnConfigureExecuteReceiveEndpoint;
```
