---

title: TypedScopedConsumeContextProvider

---

# TypedScopedConsumeContextProvider

Namespace: MassTransit.DependencyInjection

```csharp
public class TypedScopedConsumeContextProvider : ScopedConsumeContextProvider, IScopedConsumeContextProvider
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedConsumeContextProvider](../masstransit-dependencyinjection/scopedconsumecontextprovider) → [TypedScopedConsumeContextProvider](../masstransit-dependencyinjection/typedscopedconsumecontextprovider)<br/>
Implements [IScopedConsumeContextProvider](../masstransit-dependencyinjection/iscopedconsumecontextprovider)

## Properties

### **HasContext**

```csharp
public bool HasContext { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Constructors

### **TypedScopedConsumeContextProvider(IScopedConsumeContextProvider)**

```csharp
public TypedScopedConsumeContextProvider(IScopedConsumeContextProvider global)
```

#### Parameters

`global` [IScopedConsumeContextProvider](../masstransit-dependencyinjection/iscopedconsumecontextprovider)<br/>

## Methods

### **PushContext(ConsumeContext)**

```csharp
public IDisposable PushContext(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
