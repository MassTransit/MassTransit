---

title: IOptionsSet

---

# IOptionsSet

Namespace: MassTransit.Configuration

```csharp
public interface IOptionsSet
```

## Methods

### **Options\<T\>(Action\<T\>)**

Configure the options, adding the option type if it is not present

```csharp
T Options<T>(Action<T> configure)
```

#### Type Parameters

`T`<br/>
The option type

#### Parameters

`configure` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

T<br/>

### **Options\<T\>(T, Action\<T\>)**

Specify the options, will fault if it already exists

```csharp
T Options<T>(T options, Action<T> configure)
```

#### Type Parameters

`T`<br/>
The option type

#### Parameters

`options` T<br/>

`configure` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

T<br/>

### **TryGetOptions\<T\>(T)**

Return the options, if present

```csharp
bool TryGetOptions<T>(out T options)
```

#### Type Parameters

`T`<br/>
The option type

#### Parameters

`options` T<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **SelectOptions\<T\>()**

Enumerate the options which are assignable to the specified type

```csharp
IEnumerable<T> SelectOptions<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
