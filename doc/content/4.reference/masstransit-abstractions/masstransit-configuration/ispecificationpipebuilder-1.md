---

title: ISpecificationPipeBuilder<T>

---

# ISpecificationPipeBuilder\<T\>

Namespace: MassTransit.Configuration

```csharp
public interface ISpecificationPipeBuilder<T> : IPipeBuilder<T>
```

#### Type Parameters

`T`<br/>

Implements [IPipeBuilder\<T\>](../masstransit-configuration/ipipebuilder-1)

## Properties

### **IsDelegated**

If true, this is a delegated builder, and implemented message types
 and/or topology items should not be applied

```csharp
public abstract bool IsDelegated { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsImplemented**

If true, this is a builder for implemented types, so don't go down
 the rabbit hole twice.

```csharp
public abstract bool IsImplemented { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **CreateDelegatedBuilder()**

```csharp
ISpecificationPipeBuilder<T> CreateDelegatedBuilder()
```

#### Returns

[ISpecificationPipeBuilder\<T\>](../masstransit-configuration/ispecificationpipebuilder-1)<br/>

### **CreateImplementedBuilder()**

```csharp
ISpecificationPipeBuilder<T> CreateImplementedBuilder()
```

#### Returns

[ISpecificationPipeBuilder\<T\>](../masstransit-configuration/ispecificationpipebuilder-1)<br/>
