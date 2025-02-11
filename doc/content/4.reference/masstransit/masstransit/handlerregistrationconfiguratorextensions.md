---

title: HandlerRegistrationConfiguratorExtensions

---

# HandlerRegistrationConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class HandlerRegistrationConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [HandlerRegistrationConfiguratorExtensions](../masstransit/handlerregistrationconfiguratorextensions)

## Methods

### **AddHandler\<T\>(IRegistrationConfigurator)**

Adds an empty message handler, which consumes the messages and does nothing else. Useful with the test harness to ensure
 that produced messages are consumed, which can then be asserted in unit tests.

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T>(IRegistrationConfigurator configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T\>(IRegistrationConfigurator, Func\<ConsumeContext\<T\>, Task\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T>(IRegistrationConfigurator configurator, Func<ConsumeContext<T>, Task> handler)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<ConsumeContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T\>(IRegistrationConfigurator, Func\<T, Task\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T>(IRegistrationConfigurator configurator, Func<T, Task> handler)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<T, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, TResponse\>(IRegistrationConfigurator, Func\<ConsumeContext\<T\>, Task\<TResponse\>\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, TResponse>(IRegistrationConfigurator configurator, Func<ConsumeContext<T>, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<ConsumeContext\<T\>, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, TResponse\>(IRegistrationConfigurator, Func\<T, Task\<TResponse\>\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, TResponse>(IRegistrationConfigurator configurator, Func<T, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<T, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1\>(IRegistrationConfigurator, Func\<ConsumeContext\<T\>, T1, Task\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1>(IRegistrationConfigurator configurator, Func<ConsumeContext<T>, T1, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, TResponse\>(IRegistrationConfigurator, Func\<ConsumeContext\<T\>, T1, Task\<TResponse\>\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, TResponse>(IRegistrationConfigurator configurator, Func<ConsumeContext<T>, T1, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1\>(IRegistrationConfigurator, Func\<T, T1, Task\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1>(IRegistrationConfigurator configurator, Func<T, T1, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<T, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, TResponse\>(IRegistrationConfigurator, Func\<T, T1, Task\<TResponse\>\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, TResponse>(IRegistrationConfigurator configurator, Func<T, T1, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<T, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, T2\>(IRegistrationConfigurator, Func\<ConsumeContext\<T\>, T1, T2, Task\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2>(IRegistrationConfigurator configurator, Func<ConsumeContext<T>, T1, T2, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, T2, TResponse\>(IRegistrationConfigurator, Func\<ConsumeContext\<T\>, T1, T2, Task\<TResponse\>\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, TResponse>(IRegistrationConfigurator configurator, Func<ConsumeContext<T>, T1, T2, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, T2\>(IRegistrationConfigurator, Func\<T, T1, T2, Task\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2>(IRegistrationConfigurator configurator, Func<T, T1, T2, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<T, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, T2, TResponse\>(IRegistrationConfigurator, Func\<T, T1, T2, Task\<TResponse\>\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, TResponse>(IRegistrationConfigurator configurator, Func<T, T1, T2, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<T, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, T2, T3\>(IRegistrationConfigurator, Func\<ConsumeContext\<T\>, T1, T2, T3, Task\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, T3>(IRegistrationConfigurator configurator, Func<ConsumeContext<T>, T1, T2, T3, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, T2, T3, TResponse\>(IRegistrationConfigurator, Func\<ConsumeContext\<T\>, T1, T2, T3, Task\<TResponse\>\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, T3, TResponse>(IRegistrationConfigurator configurator, Func<ConsumeContext<T>, T1, T2, T3, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, T3, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, T2, T3\>(IRegistrationConfigurator, Func\<T, T1, T2, T3, Task\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, T3>(IRegistrationConfigurator configurator, Func<T, T1, T2, T3, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<T, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>

### **AddHandler\<T, T1, T2, T3, TResponse\>(IRegistrationConfigurator, Func\<T, T1, T2, T3, Task\<TResponse\>\>)**

Adds a method handler, using the first parameter to determine the message type

```csharp
public static IConsumerRegistrationConfigurator AddHandler<T, T1, T2, T3, TResponse>(IRegistrationConfigurator configurator, Func<T, T1, T2, T3, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

`TResponse`<br/>

#### Parameters

`configurator` [IRegistrationConfigurator](../masstransit/iregistrationconfigurator)<br/>

`handler` [Func\<T, T1, T2, T3, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>
An asynchronous method to handle the message

#### Returns

[IConsumerRegistrationConfigurator](../../masstransit-abstractions/masstransit/iconsumerregistrationconfigurator)<br/>
