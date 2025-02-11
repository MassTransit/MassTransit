---

title: Pipe

---

# Pipe

Namespace: MassTransit

```csharp
public static class Pipe
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Pipe](../masstransit/pipe)

## Methods

### **New\<T\>(Action\<IPipeConfigurator\<T\>\>)**

Create a new pipe using the pipe configurator to add filters, etc.

```csharp
public static IPipe<T> New<T>(Action<IPipeConfigurator<T>> callback)
```

#### Type Parameters

`T`<br/>
The pipe context type

#### Parameters

`callback` [Action\<IPipeConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

#### Returns

[IPipe\<T\>](../masstransit/ipipe-1)<br/>
An initialized pipe ready for use

### **New\<T\>(Action\<IPipeConfigurator\<T\>\>, Boolean)**

Create a new pipe using the pipe configurator to add filters, etc.

```csharp
public static IPipe<T> New<T>(Action<IPipeConfigurator<T>> callback, bool validate)
```

#### Type Parameters

`T`<br/>
The pipe context type

#### Parameters

`callback` [Action\<IPipeConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

`validate` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the pipe should be validated

#### Returns

[IPipe\<T\>](../masstransit/ipipe-1)<br/>
An initialized pipe ready for use

### **Execute\<T\>(Action\<T\>)**

Constructs a simple pipe that executes the specified action

```csharp
public static IPipe<T> Execute<T>(Action<T> action)
```

#### Type Parameters

`T`<br/>
The pipe context type

#### Parameters

`action` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The method to execute

#### Returns

[IPipe\<T\>](../masstransit/ipipe-1)<br/>
The constructed pipe

### **AddCallback\<T\>(IPipe\<T\>, Action\<T\>)**

Constructs a simple pipe that executes the specified action

```csharp
public static IPipe<T> AddCallback<T>(IPipe<T> pipe, Action<T> action)
```

#### Type Parameters

`T`<br/>
The pipe context type

#### Parameters

`pipe` [IPipe\<T\>](../masstransit/ipipe-1)<br/>

`action` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The method to execute

#### Returns

[IPipe\<T\>](../masstransit/ipipe-1)<br/>
The constructed pipe

### **ExecuteAsync\<T\>(Func\<T, Task\>)**

Constructs a simple pipe that executes the specified action

```csharp
public static IPipe<T> ExecuteAsync<T>(Func<T, Task> action)
```

#### Type Parameters

`T`<br/>
The pipe context type

#### Parameters

`action` [Func\<T, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The method to execute

#### Returns

[IPipe\<T\>](../masstransit/ipipe-1)<br/>
The constructed pipe

### **Empty\<T\>()**

Returns an empty pipe of the specified context type

```csharp
public static IPipe<T> Empty<T>()
```

#### Type Parameters

`T`<br/>
The context type

#### Returns

[IPipe\<T\>](../masstransit/ipipe-1)<br/>

### **ToPipe\<T\>(IFilter\<T\>)**

Returns a pipe for the filter

```csharp
public static IPipe<T> ToPipe<T>(IFilter<T> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`filter` [IFilter\<T\>](../masstransit/ifilter-1)<br/>

#### Returns

[IPipe\<T\>](../masstransit/ipipe-1)<br/>

#### Exceptions

[ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br/>
