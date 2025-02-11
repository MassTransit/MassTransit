---

title: IScopedConsumeContextProvider

---

# IScopedConsumeContextProvider

Namespace: MassTransit.DependencyInjection

```csharp
public interface IScopedConsumeContextProvider
```

## Properties

### **HasContext**

```csharp
public abstract bool HasContext { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **GetContext()**

```csharp
ConsumeContext GetContext()
```

#### Returns

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

### **PushContext(ConsumeContext)**

```csharp
IDisposable PushContext(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
