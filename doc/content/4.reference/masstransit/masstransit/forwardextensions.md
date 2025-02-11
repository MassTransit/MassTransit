---

title: ForwardExtensions

---

# ForwardExtensions

Namespace: MassTransit

```csharp
public static class ForwardExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ForwardExtensions](../masstransit/forwardextensions)

## Methods

### **Forward\<T\>(ConsumeContext\<T\>, Uri)**

```csharp
public static Task Forward<T>(ConsumeContext<T> context, Uri address)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`address` Uri<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Forward\<T\>(ConsumeContext\<T\>, Uri, IPipe\<SendContext\<T\>\>)**

```csharp
public static Task Forward<T>(ConsumeContext<T> context, Uri address, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`address` Uri<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Forward\<T\>(ConsumeContext\<T\>, ISendEndpoint)**

```csharp
public static Task Forward<T>(ConsumeContext<T> context, ISendEndpoint endpoint)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Forward\<T\>(ConsumeContext\<T\>, ISendEndpoint, IPipe\<SendContext\<T\>\>)**

```csharp
public static Task Forward<T>(ConsumeContext<T> context, ISendEndpoint endpoint, IPipe<SendContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext\<T\>](../../masstransit-abstractions/masstransit/consumecontext-1)<br/>

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>

`pipe` [IPipe\<SendContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Forward\<T\>(ConsumeContext, Uri, T)**

```csharp
public static Task Forward<T>(ConsumeContext context, Uri address, T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`address` Uri<br/>

`message` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Forward\<T\>(ConsumeContext, ISendEndpoint, T)**

Forward the message to another consumer

```csharp
public static Task Forward<T>(ConsumeContext context, ISendEndpoint endpoint, T message)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [ConsumeContext](../../masstransit-abstractions/masstransit/consumecontext)<br/>

`endpoint` [ISendEndpoint](../../masstransit-abstractions/masstransit/isendendpoint)<br/>
The destination endpoint

`message` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
