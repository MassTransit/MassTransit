---

title: PipeConfigurator<TContext>

---

# PipeConfigurator\<TContext\>

Namespace: MassTransit.Configuration

```csharp
public class PipeConfigurator<TContext> : IBuildPipeConfigurator<TContext>, IPipeConfigurator<TContext>, ISpecification
```

#### Type Parameters

`TContext`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PipeConfigurator\<TContext\>](../masstransit-configuration/pipeconfigurator-1)<br/>
Implements [IBuildPipeConfigurator\<TContext\>](../masstransit-configuration/ibuildpipeconfigurator-1), [IPipeConfigurator\<TContext\>](../masstransit/ipipeconfigurator-1), [ISpecification](../masstransit/ispecification)

## Constructors

### **PipeConfigurator()**

```csharp
public PipeConfigurator()
```

## Methods

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

### **AddPipeSpecification(IPipeSpecification\<TContext\>)**

```csharp
public void AddPipeSpecification(IPipeSpecification<TContext> specification)
```

#### Parameters

`specification` [IPipeSpecification\<TContext\>](../masstransit-configuration/ipipespecification-1)<br/>

### **Build()**

```csharp
public IPipe<TContext> Build()
```

#### Returns

[IPipe\<TContext\>](../masstransit/ipipe-1)<br/>

### **Method1()**

```csharp
public void Method1()
```

### **Method2()**

```csharp
public void Method2()
```
