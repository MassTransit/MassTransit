---

title: DispatchPipeSpecification<TInput>

---

# DispatchPipeSpecification\<TInput\>

Namespace: MassTransit.Configuration

```csharp
public class DispatchPipeSpecification<TInput> : IPipeSpecification<TInput>, ISpecification, IDispatchConfigurator<TInput>
```

#### Type Parameters

`TInput`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DispatchPipeSpecification\<TInput\>](../masstransit-configuration/dispatchpipespecification-1)<br/>
Implements [IPipeSpecification\<TInput\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification), [IDispatchConfigurator\<TInput\>](../masstransit/idispatchconfigurator-1)

## Constructors

### **DispatchPipeSpecification(IPipeContextConverterFactory\<TInput\>)**

```csharp
public DispatchPipeSpecification(IPipeContextConverterFactory<TInput> pipeContextConverterFactory)
```

#### Parameters

`pipeContextConverterFactory` [IPipeContextConverterFactory\<TInput\>](../masstransit-middleware/ipipecontextconverterfactory-1)<br/>

## Methods

### **Pipe\<T\>(Action\<IPipeConfigurator\<T\>\>)**

```csharp
public void Pipe<T>(Action<IPipeConfigurator<T>> configurePipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurePipe` [Action\<IPipeConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **Apply(IPipeBuilder\<TInput\>)**

```csharp
public void Apply(IPipeBuilder<TInput> builder)
```

#### Parameters

`builder` [IPipeBuilder\<TInput\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
