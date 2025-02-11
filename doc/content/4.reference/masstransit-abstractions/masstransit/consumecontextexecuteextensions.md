---

title: ConsumeContextExecuteExtensions

---

# ConsumeContextExecuteExtensions

Namespace: MassTransit

```csharp
public static class ConsumeContextExecuteExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumeContextExecuteExtensions](../masstransit/consumecontextexecuteextensions)

## Methods

### **ToPipe\<T\>(Action\<ConsumeContext\<T\>\>)**

```csharp
public static IPipe<ConsumeContext<T>> ToPipe<T>(Action<ConsumeContext<T>> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Action\<ConsumeContext\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<ConsumeContext\<T\>\>](../masstransit/ipipe-1)<br/>

### **ToPipe\<T\>(Func\<ConsumeContext\<T\>, Task\>)**

```csharp
public static IPipe<ConsumeContext<T>> ToPipe<T>(Func<ConsumeContext<T>, Task> callback)
```

#### Type Parameters

`T`<br/>

#### Parameters

`callback` [Func\<ConsumeContext\<T\>, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IPipe\<ConsumeContext\<T\>\>](../masstransit/ipipe-1)<br/>

### **ToPipe(Action\<ConsumeContext\>)**

```csharp
public static IPipe<ConsumeContext> ToPipe(Action<ConsumeContext> callback)
```

#### Parameters

`callback` [Action\<ConsumeContext\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

[IPipe\<ConsumeContext\>](../masstransit/ipipe-1)<br/>

### **ToPipe(Func\<ConsumeContext, Task\>)**

```csharp
public static IPipe<ConsumeContext> ToPipe(Func<ConsumeContext, Task> callback)
```

#### Parameters

`callback` [Func\<ConsumeContext, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[IPipe\<ConsumeContext\>](../masstransit/ipipe-1)<br/>
