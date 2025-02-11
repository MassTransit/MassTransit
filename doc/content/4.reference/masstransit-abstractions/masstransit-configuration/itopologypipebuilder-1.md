---

title: ITopologyPipeBuilder<T>

---

# ITopologyPipeBuilder\<T\>

Namespace: MassTransit.Configuration

A pipe builder used by topologies, which indicates whether the message type
 is either delegated (called from a sub-specification) or implemented (being called
 when the actual type is a subtype and this is an implemented type).

```csharp
public interface ITopologyPipeBuilder<T> : IPipeBuilder<T>
```

#### Type Parameters

`T`<br/>
The pipe context type

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

Creates a new builder where the Delegated flag is true

```csharp
ITopologyPipeBuilder<T> CreateDelegatedBuilder()
```

#### Returns

[ITopologyPipeBuilder\<T\>](../masstransit-configuration/itopologypipebuilder-1)<br/>
