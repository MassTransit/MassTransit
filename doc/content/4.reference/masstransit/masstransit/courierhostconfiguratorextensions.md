---

title: CourierHostConfiguratorExtensions

---

# CourierHostConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class CourierHostConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CourierHostConfiguratorExtensions](../masstransit/courierhostconfiguratorextensions)

## Methods

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Uri, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Uri compensateAddress, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateAddress` Uri<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Uri, Func\<TActivity\>, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Uri compensateAddress, Func<TActivity> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateAddress` Uri<br/>

`activityFactory` [Func\<TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Func\<TActivity\>, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Func<TActivity> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`activityFactory` [Func\<TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Uri, Func\<TArguments, TActivity\>, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Uri compensateAddress, Func<TArguments, TActivity> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateAddress` Uri<br/>

`activityFactory` [Func\<TArguments, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Func\<TArguments, TActivity\>, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Func<TArguments, TActivity> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`activityFactory` [Func\<TArguments, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, Uri, IExecuteActivityFactory\<TActivity, TArguments\>, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, Uri compensateAddress, IExecuteActivityFactory<TActivity, TArguments> factory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`compensateAddress` Uri<br/>

`factory` [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **ExecuteActivityHost\<TActivity, TArguments\>(IReceiveEndpointConfigurator, IExecuteActivityFactory\<TActivity, TArguments\>, Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>)**

```csharp
public static void ExecuteActivityHost<TActivity, TArguments>(IReceiveEndpointConfigurator configurator, IExecuteActivityFactory<TActivity, TArguments> factory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TArguments`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`factory` [IExecuteActivityFactory\<TActivity, TArguments\>](../../masstransit-abstractions/masstransit/iexecuteactivityfactory-2)<br/>

`configure` [Action\<IExecuteActivityConfigurator\<TActivity, TArguments\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CompensateActivityHost\<TActivity, TLog\>(IReceiveEndpointConfigurator, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

```csharp
public static void CompensateActivityHost<TActivity, TLog>(IReceiveEndpointConfigurator configurator, Action<ICompensateActivityConfigurator<TActivity, TLog>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`configure` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CompensateActivityHost\<TActivity, TLog\>(IReceiveEndpointConfigurator, Func\<TActivity\>, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

```csharp
public static void CompensateActivityHost<TActivity, TLog>(IReceiveEndpointConfigurator configurator, Func<TActivity> activityFactory, Action<ICompensateActivityConfigurator<TActivity, TLog>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`activityFactory` [Func\<TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>

`configure` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CompensateActivityHost\<TActivity, TLog\>(IReceiveEndpointConfigurator, Func\<TLog, TActivity\>, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

```csharp
public static void CompensateActivityHost<TActivity, TLog>(IReceiveEndpointConfigurator configurator, Func<TLog, TActivity> activityFactory, Action<ICompensateActivityConfigurator<TActivity, TLog>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`activityFactory` [Func\<TLog, TActivity\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

`configure` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **CompensateActivityHost\<TActivity, TLog\>(IReceiveEndpointConfigurator, ICompensateActivityFactory\<TActivity, TLog\>, Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>)**

```csharp
public static void CompensateActivityHost<TActivity, TLog>(IReceiveEndpointConfigurator configurator, ICompensateActivityFactory<TActivity, TLog> factory, Action<ICompensateActivityConfigurator<TActivity, TLog>> configure)
```

#### Type Parameters

`TActivity`<br/>

`TLog`<br/>

#### Parameters

`configurator` [IReceiveEndpointConfigurator](../../masstransit-abstractions/masstransit/ireceiveendpointconfigurator)<br/>

`factory` [ICompensateActivityFactory\<TActivity, TLog\>](../../masstransit-abstractions/masstransit/icompensateactivityfactory-2)<br/>

`configure` [Action\<ICompensateActivityConfigurator\<TActivity, TLog\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
