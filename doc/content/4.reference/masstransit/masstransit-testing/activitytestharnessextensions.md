---

title: ActivityTestHarnessExtensions

---

# ActivityTestHarnessExtensions

Namespace: MassTransit.Testing

```csharp
public static class ActivityTestHarnessExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ActivityTestHarnessExtensions](../masstransit-testing/activitytestharnessextensions)

## Methods

### **Activity\<TActivity, TArguments, TLog\>(BusTestHarness)**

Creates an activity test harness

```csharp
public static ActivityTestHarness<TActivity, TArguments, TLog> Activity<TActivity, TArguments, TLog>(BusTestHarness harness)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

#### Returns

[ActivityTestHarness\<TActivity, TArguments, TLog\>](../masstransit-testing/activitytestharness-3)<br/>

### **Activity\<TActivity, TArguments, TLog\>(BusTestHarness, Func\<TArguments, TActivity\>, Func\<TLog, TActivity\>)**

Creates an activity test harness

```csharp
public static ActivityTestHarness<TActivity, TArguments, TLog> Activity<TActivity, TArguments, TLog>(BusTestHarness harness, Func<TArguments, TActivity> executeFactoryMethod, Func<TLog, TActivity> compensateFactoryMethod)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

`TLog`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`executeFactoryMethod` [Func\<TArguments, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`compensateFactoryMethod` [Func\<TLog, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ActivityTestHarness\<TActivity, TArguments, TLog\>](../masstransit-testing/activitytestharness-3)<br/>

### **ExecuteActivity\<TActivity, TArguments\>(BusTestHarness)**

Creates an execute-only activity test harness

```csharp
public static ExecuteActivityTestHarness<TActivity, TArguments> ExecuteActivity<TActivity, TArguments>(BusTestHarness harness)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

#### Returns

[ExecuteActivityTestHarness\<TActivity, TArguments\>](../masstransit-testing/executeactivitytestharness-2)<br/>

### **ExecuteActivity\<TActivity, TArguments\>(BusTestHarness, Func\<TArguments, TActivity\>)**

Creates an execute-only activity test harness

```csharp
public static ExecuteActivityTestHarness<TActivity, TArguments> ExecuteActivity<TActivity, TArguments>(BusTestHarness harness, Func<TArguments, TActivity> executeFactoryMethod)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`harness` [BusTestHarness](../masstransit-testing/bustestharness)<br/>

`executeFactoryMethod` [Func\<TArguments, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[ExecuteActivityTestHarness\<TActivity, TArguments\>](../masstransit-testing/executeactivitytestharness-2)<br/>
