---

title: OptionsSet

---

# OptionsSet

Namespace: MassTransit.Configuration

```csharp
public class OptionsSet : IOptionsSet
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OptionsSet](../masstransit-configuration/optionsset)<br/>
Implements [IOptionsSet](../masstransit-configuration/ioptionsset)

## Constructors

### **OptionsSet()**

```csharp
public OptionsSet()
```

## Methods

### **Options\<T\>(Action\<T\>)**

Configure the options, adding the option type if it is not present

```csharp
public T Options<T>(Action<T> configure)
```

#### Type Parameters

`T`<br/>
The option type

#### Parameters

`configure` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

#### Returns

T<br/>

### **Options\<T\>(T, Action\<T\>)**

Configure the options, adding the option type if it is not present

```csharp
public T Options<T>(T options, Action<T> configure)
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
public bool TryGetOptions<T>(out T options)
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
public IEnumerable<T> SelectOptions<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **ValidateOptions()**

Enumerate the options which are assignable to the specified type

```csharp
protected IEnumerable<ValidationResult> ValidateOptions()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
