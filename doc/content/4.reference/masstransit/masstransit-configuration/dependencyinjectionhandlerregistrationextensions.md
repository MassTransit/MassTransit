---

title: DependencyInjectionHandlerRegistrationExtensions

---

# DependencyInjectionHandlerRegistrationExtensions

Namespace: MassTransit.Configuration

```csharp
public static class DependencyInjectionHandlerRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionHandlerRegistrationExtensions](../masstransit-configuration/dependencyinjectionhandlerregistrationextensions)

## Methods

### **RegisterHandler\<T\>(IServiceCollection)**

```csharp
public static IConsumerRegistration RegisterHandler<T>(IServiceCollection collection)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T\>(IServiceCollection, IContainerRegistrar)**

```csharp
public static IConsumerRegistration RegisterHandler<T>(IServiceCollection collection, IContainerRegistrar registrar)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T\>(IServiceCollection, Func\<ConsumeContext\<T\>, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T>(IServiceCollection collection, Func<ConsumeContext<T>, Task> handler)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<ConsumeContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T\>(IServiceCollection, IContainerRegistrar, Func\<ConsumeContext\<T\>, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T>(IServiceCollection collection, IContainerRegistrar registrar, Func<ConsumeContext<T>, Task> handler)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<ConsumeContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T\>(IServiceCollection, Func\<T, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T>(IServiceCollection collection, Func<T, Task> handler)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<T, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T\>(IServiceCollection, IContainerRegistrar, Func\<T, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T>(IServiceCollection collection, IContainerRegistrar registrar, Func<T, Task> handler)
```

#### Type Parameters

`T`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<T, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, TResponse\>(IServiceCollection, Func\<ConsumeContext\<T\>, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, TResponse>(IServiceCollection collection, Func<ConsumeContext<T>, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<ConsumeContext\<T\>, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, TResponse\>(IServiceCollection, IContainerRegistrar, Func\<ConsumeContext\<T\>, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, TResponse>(IServiceCollection collection, IContainerRegistrar registrar, Func<ConsumeContext<T>, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<ConsumeContext\<T\>, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, TResponse\>(IServiceCollection, Func\<T, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, TResponse>(IServiceCollection collection, Func<T, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<T, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, TResponse\>(IServiceCollection, IContainerRegistrar, Func\<T, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, TResponse>(IServiceCollection collection, IContainerRegistrar registrar, Func<T, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<T, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1\>(IServiceCollection, Func\<ConsumeContext\<T\>, T1, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1>(IServiceCollection collection, Func<ConsumeContext<T>, T1, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1\>(IServiceCollection, IContainerRegistrar, Func\<ConsumeContext\<T\>, T1, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1>(IServiceCollection collection, IContainerRegistrar registrar, Func<ConsumeContext<T>, T1, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1\>(IServiceCollection, Func\<T, T1, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1>(IServiceCollection collection, Func<T, T1, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<T, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1\>(IServiceCollection, IContainerRegistrar, Func\<T, T1, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1>(IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<T, T1, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, TResponse\>(IServiceCollection, Func\<ConsumeContext\<T\>, T1, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, TResponse>(IServiceCollection collection, Func<ConsumeContext<T>, T1, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, TResponse\>(IServiceCollection, IContainerRegistrar, Func\<ConsumeContext\<T\>, T1, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, TResponse>(IServiceCollection collection, IContainerRegistrar registrar, Func<ConsumeContext<T>, T1, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, TResponse\>(IServiceCollection, Func\<T, T1, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, TResponse>(IServiceCollection collection, Func<T, T1, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<T, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, TResponse\>(IServiceCollection, IContainerRegistrar, Func\<T, T1, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, TResponse>(IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<T, T1, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-3)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2\>(IServiceCollection, Func\<ConsumeContext\<T\>, T1, T2, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2>(IServiceCollection collection, Func<ConsumeContext<T>, T1, T2, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2\>(IServiceCollection, IContainerRegistrar, Func\<ConsumeContext\<T\>, T1, T2, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2>(IServiceCollection collection, IContainerRegistrar registrar, Func<ConsumeContext<T>, T1, T2, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2\>(IServiceCollection, Func\<T, T1, T2, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2>(IServiceCollection collection, Func<T, T1, T2, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<T, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2\>(IServiceCollection, IContainerRegistrar, Func\<T, T1, T2, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2>(IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, T2, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<T, T1, T2, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, TResponse\>(IServiceCollection, Func\<ConsumeContext\<T\>, T1, T2, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, TResponse>(IServiceCollection collection, Func<ConsumeContext<T>, T1, T2, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, TResponse\>(IServiceCollection, IContainerRegistrar, Func\<ConsumeContext\<T\>, T1, T2, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, TResponse>(IServiceCollection collection, IContainerRegistrar registrar, Func<ConsumeContext<T>, T1, T2, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, TResponse\>(IServiceCollection, Func\<T, T1, T2, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, TResponse>(IServiceCollection collection, Func<T, T1, T2, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<T, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, TResponse\>(IServiceCollection, IContainerRegistrar, Func\<T, T1, T2, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, TResponse>(IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, T2, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<T, T1, T2, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-4)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, T3\>(IServiceCollection, Func\<ConsumeContext\<T\>, T1, T2, T3, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, T3>(IServiceCollection collection, Func<ConsumeContext<T>, T1, T2, T3, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, T3\>(IServiceCollection, IContainerRegistrar, Func\<ConsumeContext\<T\>, T1, T2, T3, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, T3>(IServiceCollection collection, IContainerRegistrar registrar, Func<ConsumeContext<T>, T1, T2, T3, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, T3\>(IServiceCollection, Func\<T, T1, T2, T3, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, T3>(IServiceCollection collection, Func<T, T1, T2, T3, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<T, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, T3\>(IServiceCollection, IContainerRegistrar, Func\<T, T1, T2, T3, Task\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, T3>(IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, T2, T3, Task> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<T, T1, T2, T3, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, T3, TResponse\>(IServiceCollection, Func\<ConsumeContext\<T\>, T1, T2, T3, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, T3, TResponse>(IServiceCollection collection, Func<ConsumeContext<T>, T1, T2, T3, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, T3, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, T3, TResponse\>(IServiceCollection, IContainerRegistrar, Func\<ConsumeContext\<T\>, T1, T2, T3, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, T3, TResponse>(IServiceCollection collection, IContainerRegistrar registrar, Func<ConsumeContext<T>, T1, T2, T3, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<ConsumeContext\<T\>, T1, T2, T3, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, T3, TResponse\>(IServiceCollection, Func\<T, T1, T2, T3, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, T3, TResponse>(IServiceCollection collection, Func<T, T1, T2, T3, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`handler` [Func\<T, T1, T2, T3, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>

### **RegisterHandler\<T, T1, T2, T3, TResponse\>(IServiceCollection, IContainerRegistrar, Func\<T, T1, T2, T3, Task\<TResponse\>\>)**

```csharp
public static IConsumerRegistration RegisterHandler<T, T1, T2, T3, TResponse>(IServiceCollection collection, IContainerRegistrar registrar, Func<T, T1, T2, T3, Task<TResponse>> handler)
```

#### Type Parameters

`T`<br/>

`T1`<br/>

`T2`<br/>

`T3`<br/>

`TResponse`<br/>

#### Parameters

`collection` IServiceCollection<br/>

`registrar` [IContainerRegistrar](../masstransit-configuration/icontainerregistrar)<br/>

`handler` [Func\<T, T1, T2, T3, Task\<TResponse\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-5)<br/>

#### Returns

[IConsumerRegistration](../masstransit-configuration/iconsumerregistration)<br/>
