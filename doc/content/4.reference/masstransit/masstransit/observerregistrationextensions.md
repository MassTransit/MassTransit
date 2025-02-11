---

title: ObserverRegistrationExtensions

---

# ObserverRegistrationExtensions

Namespace: MassTransit

```csharp
public static class ObserverRegistrationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ObserverRegistrationExtensions](../masstransit/observerregistrationextensions)

## Methods

### **AddBusObserver\<T\>(IServiceCollection)**

Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddBusObserver<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddBusObserver\<T\>(IServiceCollection, Func\<IServiceProvider, T\>)**

Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddBusObserver<T>(IServiceCollection services, Func<IServiceProvider, T> factory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

`factory` [Func\<IServiceProvider, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

IServiceCollection<br/>

### **AddReceiveEndpointObserver\<T\>(IServiceCollection)**

Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddReceiveEndpointObserver<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddReceiveEndpointObserver\<T\>(IServiceCollection, Func\<IServiceProvider, T\>)**

Add a receive endpoint observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddReceiveEndpointObserver<T>(IServiceCollection services, Func<IServiceProvider, T> factory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

`factory` [Func\<IServiceProvider, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

IServiceCollection<br/>

### **AddReceiveObserver\<T\>(IServiceCollection)**

Add a receive observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddReceiveObserver<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddReceiveObserver\<T\>(IServiceCollection, Func\<IServiceProvider, T\>)**

Add a receive observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddReceiveObserver<T>(IServiceCollection services, Func<IServiceProvider, T> factory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

`factory` [Func\<IServiceProvider, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

IServiceCollection<br/>

### **AddConsumeObserver\<T\>(IServiceCollection)**

Add a consume observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddConsumeObserver<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddConsumeObserver\<T\>(IServiceCollection, Func\<IServiceProvider, T\>)**

Add a consume observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddConsumeObserver<T>(IServiceCollection services, Func<IServiceProvider, T> factory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

`factory` [Func\<IServiceProvider, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

IServiceCollection<br/>

### **AddSendObserver\<T\>(IServiceCollection)**

Add a send observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddSendObserver<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddSendObserver\<T\>(IServiceCollection, Func\<IServiceProvider, T\>)**

Add a send observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddSendObserver<T>(IServiceCollection services, Func<IServiceProvider, T> factory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

`factory` [Func\<IServiceProvider, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

IServiceCollection<br/>

### **AddPublishObserver\<T\>(IServiceCollection)**

Add a publish observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddPublishObserver<T>(IServiceCollection services)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddPublishObserver\<T\>(IServiceCollection, Func\<IServiceProvider, T\>)**

Add a publish observer to the container, which will be resolved and connected to the bus by the container

```csharp
public static IServiceCollection AddPublishObserver<T>(IServiceCollection services, Func<IServiceProvider, T> factory)
```

#### Type Parameters

`T`<br/>

#### Parameters

`services` IServiceCollection<br/>

`factory` [Func\<IServiceProvider, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

IServiceCollection<br/>

### **AddEventObserver\<TInstance, T\>(IServiceCollection)**

Add a saga state machine event observer to the container, which will be resolved and connected to the state machine by the container

```csharp
public static IServiceCollection AddEventObserver<TInstance, T>(IServiceCollection services)
```

#### Type Parameters

`TInstance`<br/>
The saga state machine instance type

`T`<br/>
The event observer type

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddEventObserver\<TInstance, T\>(IServiceCollection, Func\<IServiceProvider, T\>)**

Add a saga state machine event observer to the container, which will be resolved and connected to the state machine by the container

```csharp
public static IServiceCollection AddEventObserver<TInstance, T>(IServiceCollection services, Func<IServiceProvider, T> factory)
```

#### Type Parameters

`TInstance`<br/>
The saga state machine instance type

`T`<br/>
The event observer type

#### Parameters

`services` IServiceCollection<br/>

`factory` [Func\<IServiceProvider, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

IServiceCollection<br/>

### **AddStateObserver\<TInstance, T\>(IServiceCollection)**

Add a saga state machine state observer to the container, which will be resolved and connected to the state machine by the container

```csharp
public static IServiceCollection AddStateObserver<TInstance, T>(IServiceCollection services)
```

#### Type Parameters

`TInstance`<br/>
The saga state machine instance type

`T`<br/>
The event observer type

#### Parameters

`services` IServiceCollection<br/>

#### Returns

IServiceCollection<br/>

### **AddStateObserver\<TInstance, T\>(IServiceCollection, Func\<IServiceProvider, T\>)**

Add a saga state machine state observer to the container, which will be resolved and connected to the state machine by the container

```csharp
public static IServiceCollection AddStateObserver<TInstance, T>(IServiceCollection services, Func<IServiceProvider, T> factory)
```

#### Type Parameters

`TInstance`<br/>
The saga state machine instance type

`T`<br/>
The event observer type

#### Parameters

`services` IServiceCollection<br/>

`factory` [Func\<IServiceProvider, T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

IServiceCollection<br/>
