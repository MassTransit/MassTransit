---

title: ConsumerConvention

---

# ConsumerConvention

Namespace: MassTransit

Used to register conventions for consumer message types

```csharp
public static class ConsumerConvention
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerConvention](../masstransit/consumerconvention)

## Methods

### **Register\<T\>()**

Register a consumer convention to be used for finding message types

```csharp
public static bool Register<T>()
```

#### Type Parameters

`T`<br/>
The convention type

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Register\<T\>(T)**

Register a consumer convention to be used for finding message types

```csharp
public static bool Register<T>(T convention)
```

#### Type Parameters

`T`<br/>
The convention type

#### Parameters

`convention` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Remove\<T\>()**

Remove a consumer convention used for finding message types

```csharp
public static void Remove<T>()
```

#### Type Parameters

`T`<br/>
The convention type to remove
