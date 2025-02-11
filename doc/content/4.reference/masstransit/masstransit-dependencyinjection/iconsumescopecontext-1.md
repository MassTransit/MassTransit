---

title: IConsumeScopeContext<TMessage>

---

# IConsumeScopeContext\<TMessage\>

Namespace: MassTransit.DependencyInjection

```csharp
public interface IConsumeScopeContext<TMessage> : IAsyncDisposable
```

#### Type Parameters

`TMessage`<br/>

Implements [IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

## Properties

### **Context**

```csharp
public abstract ConsumeContext<TMessage> Context { get; }
```

#### Property Value

[ConsumeContext\<TMessage\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

## Methods

### **GetService\<T\>()**

```csharp
T GetService<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

T<br/>

### **CreateInstance\<T\>(Object[])**

```csharp
T CreateInstance<T>(Object[] arguments)
```

#### Type Parameters

`T`<br/>

#### Parameters

`arguments` [Object[]](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

#### Returns

T<br/>

### **PushConsumeContext(ConsumeContext)**

```csharp
IDisposable PushConsumeContext(ConsumeContext context)
```

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

#### Returns

[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br/>
