---

title: IEndpointNameFormatter

---

# IEndpointNameFormatter

Namespace: MassTransit

```csharp
public interface IEndpointNameFormatter
```

## Properties

### **Separator**

The separator string used between words

```csharp
public abstract string Separator { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **TemporaryEndpoint(String)**

Generate a temporary endpoint name, containing the specified tag

```csharp
string TemporaryEndpoint(string tag)
```

#### Parameters

`tag` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Consumer\<T\>()**

```csharp
string Consumer<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Message\<T\>()**

```csharp
string Message<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Saga\<T\>()**

```csharp
string Saga<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExecuteActivity\<T, TArguments\>()**

```csharp
string ExecuteActivity<T, TArguments>()
```

#### Type Parameters

`T`<br/>

`TArguments`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **CompensateActivity\<T, TLog\>()**

```csharp
string CompensateActivity<T, TLog>()
```

#### Type Parameters

`T`<br/>

`TLog`<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **SanitizeName(String)**

Clean up a name so that it matches the formatting.
 For instance, SubmitOrderControl -&gt; submit-order-control (kebab case)

```csharp
string SanitizeName(string name)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
