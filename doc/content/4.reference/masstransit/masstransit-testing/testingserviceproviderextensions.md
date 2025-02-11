---

title: TestingServiceProviderExtensions

---

# TestingServiceProviderExtensions

Namespace: MassTransit.Testing

```csharp
public static class TestingServiceProviderExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TestingServiceProviderExtensions](../masstransit-testing/testingserviceproviderextensions)

## Methods

### **GetTestHarness(IServiceProvider)**

```csharp
public static ITestHarness GetTestHarness(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[ITestHarness](../masstransit-testing/itestharness)<br/>

### **StartTestHarness(IServiceProvider)**

```csharp
public static Task<ITestHarness> StartTestHarness(IServiceProvider provider)
```

#### Parameters

`provider` IServiceProvider<br/>

#### Returns

[Task\<ITestHarness\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **ConnectPublishHandler\<T\>(ITestHarness, Func\<ConsumeContext\<T\>, Boolean\>)**

```csharp
public static Task<Task<ConsumeContext<T>>> ConnectPublishHandler<T>(ITestHarness harness, Func<ConsumeContext<T>, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [ITestHarness](../masstransit-testing/itestharness)<br/>

`filter` [Func\<ConsumeContext\<T\>, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Task\<Task\<ConsumeContext\<T\>\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

### **AddTaskCompletionSource\<T\>(IBusRegistrationConfigurator)**

```csharp
public static void AddTaskCompletionSource<T>(IBusRegistrationConfigurator configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **Stop(ITestHarness, CancellationToken)**

Stop the test harness, which stops the bus and all hosted services that were started.

```csharp
public static Task Stop(ITestHarness harness, CancellationToken cancellationToken)
```

#### Parameters

`harness` [ITestHarness](../masstransit-testing/itestharness)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **RestartHostedServices(ITestHarness, CancellationToken)**

```csharp
public static Task RestartHostedServices(ITestHarness harness, CancellationToken cancellationToken)
```

#### Parameters

`harness` [ITestHarness](../masstransit-testing/itestharness)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **AddSagaInstance\<T\>(ITestHarness, Nullable\<Guid\>, Action\<T\>)**

Adds a saga instance to the in-memory saga repository.

```csharp
public static void AddSagaInstance<T>(ITestHarness harness, Nullable<Guid> correlationId, Action<T> callback)
```

#### Type Parameters

`T`<br/>
The saga type

#### Parameters

`harness` [ITestHarness](../masstransit-testing/itestharness)<br/>
The test harness

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
The correlationId for the newly created saga instance

`callback` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Callback to set any additional properties on the saga instance

### **AddOrUpdateSagaInstance\<T\>(ITestHarness, Nullable\<Guid\>, Action\<T\>, CancellationToken)**

Adds or updates an existing saga instance using the in-memory saga repository.

```csharp
public static Task AddOrUpdateSagaInstance<T>(ITestHarness harness, Nullable<Guid> correlationId, Action<T> callback, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [ITestHarness](../masstransit-testing/itestharness)<br/>

`correlationId` [Nullable\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

`callback` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **TryRemoveSagaInstance\<T\>(ITestHarness, Guid, CancellationToken)**

Removes a saga instance from the in-memory saga repository (if it exists).

```csharp
public static Task<bool> TryRemoveSagaInstance<T>(ITestHarness harness, Guid correlationId, CancellationToken cancellationToken)
```

#### Type Parameters

`T`<br/>

#### Parameters

`harness` [ITestHarness](../masstransit-testing/itestharness)<br/>

`correlationId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`cancellationToken` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br/>

#### Returns

[Task\<Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

#### Exceptions

[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception)<br/>
