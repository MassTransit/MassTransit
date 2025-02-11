---

title: DependencyInjectionTestingExtensions

---

# DependencyInjectionTestingExtensions

Namespace: MassTransit

```csharp
public static class DependencyInjectionTestingExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionTestingExtensions](../masstransit/dependencyinjectiontestingextensions)

## Methods

### **AddMassTransitTestHarness(IServiceCollection, Action\<IBusRegistrationConfigurator\>)**

AddMassTransit, including the test harness, to the container.
 To specify a transport, add the appropriate UsingXxx method. If a transport is not specified, the
 default in-memory transport will be used, and ConfigureEndpoints will be called.
 If MassTransit has already been configured, the existing bus configuration will be replaced with an in-memory
 configuration (by default, unless another UsingXxx transport method is specified), and saga repositories are
 replaced with in-memory as well.

```csharp
public static IServiceCollection AddMassTransitTestHarness(IServiceCollection services, Action<IBusRegistrationConfigurator> configure)
```

#### Parameters

`services` IServiceCollection<br/>

`configure` [Action\<IBusRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

IServiceCollection<br/>

### **AddMassTransitTestHarness(IServiceCollection, TextWriter, Action\<IBusRegistrationConfigurator\>)**

AddMassTransit, including the test harness, to the container.
 To specify a transport, add the appropriate UsingXxx method. If a transport is not specified, the
 default in-memory transport will be used, and ConfigureEndpoints will be called.
 If MassTransit has already been configured, the existing bus configuration will be replaced with an in-memory
 configuration (by default, unless another UsingXxx transport method is specified), and saga repositories are
 replaced with in-memory as well.

```csharp
public static IServiceCollection AddMassTransitTestHarness(IServiceCollection services, TextWriter textWriter, Action<IBusRegistrationConfigurator> configure)
```

#### Parameters

`services` IServiceCollection<br/>

`textWriter` [TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>

`configure` [Action\<IBusRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

IServiceCollection<br/>

### **AddMassTransitTextWriterLogger(IServiceCollection, TextWriter)**

Internally used by AddMassTransitTestHarness to add a console-based  for unit testing

```csharp
public static IServiceCollection AddMassTransitTextWriterLogger(IServiceCollection services, TextWriter textWriter)
```

#### Parameters

`services` IServiceCollection<br/>

`textWriter` [TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>

#### Returns

IServiceCollection<br/>

### **AddTelemetryListener(IServiceCollection, Boolean)**

Adds a telemetry listener to the test harness, which outputs a timeline view of the unit test

```csharp
public static IServiceCollection AddTelemetryListener(IServiceCollection services, bool includeDetails)
```

#### Parameters

`services` IServiceCollection<br/>

`includeDetails` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, additional details from each span are shown

#### Returns

IServiceCollection<br/>

### **AddTelemetryListener(IServiceCollection, TextWriter, Boolean)**

Adds a telemetry listener to the test harness, which outputs a timeline view of the unit test

```csharp
public static IServiceCollection AddTelemetryListener(IServiceCollection services, TextWriter textWriter, bool includeDetails)
```

#### Parameters

`services` IServiceCollection<br/>

`textWriter` [TextWriter](https://learn.microsoft.com/en-us/dotnet/api/system.io.textwriter)<br/>
Override the default Console.Out TextWriter

`includeDetails` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
If true, additional details from each span are shown

#### Returns

IServiceCollection<br/>

### **SetTestTimeouts(IBusRegistrationConfigurator, Nullable\<TimeSpan\>, Nullable\<TimeSpan\>)**

Specify the test and/or the test inactivity timeouts that should be used by the test harness.

```csharp
public static IBusRegistrationConfigurator SetTestTimeouts(IBusRegistrationConfigurator configurator, Nullable<TimeSpan> testTimeout, Nullable<TimeSpan> testInactivityTimeout)
```

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

`testTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
If specified, changes the test timeout

`testInactivityTimeout` [Nullable\<TimeSpan\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
If specified, changes the test inactivity timeout

#### Returns

[IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **AddMassTransitInMemoryTestHarness(IServiceCollection, Action\<IBusRegistrationConfigurator\>)**

#### Caution

Use AddMassTransitTestHarness instead. Visit https://masstransit.io/obsolete for details.

---

Add the In-Memory test harness to the container, and configure it using the callback specified.

```csharp
public static IServiceCollection AddMassTransitInMemoryTestHarness(IServiceCollection services, Action<IBusRegistrationConfigurator> configure)
```

#### Parameters

`services` IServiceCollection<br/>

`configure` [Action\<IBusRegistrationConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

IServiceCollection<br/>

### **AddConsumerContainerTestHarness\<T\>(IServiceCollection)**

Add a consumer test harness for the specified consumer to the container

```csharp
public static void AddConsumerContainerTestHarness<T>(IServiceCollection configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` IServiceCollection<br/>

### **AddSagaContainerTestHarness\<T\>(IServiceCollection)**

Add a saga test harness for the specified saga to the container. The saga must be added separately, including
 a valid saga repository.

```csharp
public static void AddSagaContainerTestHarness<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

### **AddSagaStateMachineContainerTestHarness\<TStateMachine, T\>(IServiceCollection)**

Add a saga state machine test harness for the specified saga to the container. The saga must be added separately, including
 a valid saga repository.

```csharp
public static void AddSagaStateMachineContainerTestHarness<TStateMachine, T>(IServiceCollection services)
```

#### Type Parameters

`TStateMachine`<br/>

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

### **AddConsumerTestHarness\<T\>(IBusRegistrationConfigurator)**

#### Caution

Use AddMassTransitTestHarness instead. Visit https://masstransit.io/obsolete for details.

---

Add a consumer test harness for the specified consumer to the container

```csharp
public static void AddConsumerTestHarness<T>(IBusRegistrationConfigurator configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **AddSagaTestHarness\<T\>(IBusRegistrationConfigurator)**

#### Caution

Use AddMassTransitTestHarness instead. Visit https://masstransit.io/obsolete for details.

---

Add a saga test harness for the specified saga to the container. The saga must be added separately, including
 a valid saga repository.

```csharp
public static void AddSagaTestHarness<T>(IBusRegistrationConfigurator configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>

### **AddSagaStateMachineTestHarness\<TStateMachine, TSaga\>(IBusRegistrationConfigurator)**

#### Caution

Use AddMassTransitTestHarness instead. Visit https://masstransit.io/obsolete for details.

---

Add a saga test harness for the specified saga to the container. The saga must be added separately, including
 a valid saga repository.

```csharp
public static void AddSagaStateMachineTestHarness<TStateMachine, TSaga>(IBusRegistrationConfigurator configurator)
```

#### Type Parameters

`TStateMachine`<br/>

`TSaga`<br/>

#### Parameters

`configurator` [IBusRegistrationConfigurator](../masstransit/ibusregistrationconfigurator)<br/>
