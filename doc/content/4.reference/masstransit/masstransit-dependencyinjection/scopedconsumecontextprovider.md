---

title: ScopedConsumeContextProvider

---

# ScopedConsumeContextProvider

Namespace: MassTransit.DependencyInjection

Captures the  for the current message as a scoped provider, so that it can be resolved
 by components at runtime (since MS DI doesn't support runtime configuration of scopes)

```csharp
public class ScopedConsumeContextProvider : IScopedConsumeContextProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedConsumeContextProvider](../masstransit-dependencyinjection/scopedconsumecontextprovider)<br/>
Implements [IScopedConsumeContextProvider](../masstransit-dependencyinjection/iscopedconsumecontextprovider)

## Properties

### **HasContext**

```csharp
public bool HasContext { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **ScopedConsumeContextProvider()**

```csharp
public ScopedConsumeContextProvider()
```

## Methods

### **PushContext(ConsumeContext)**

```csharp
public IDisposable PushContext(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>

### **GetContext()**

```csharp
public ConsumeContext GetContext()
```

#### Returns

[ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>
