---

title: CommandContext<T>

---

# CommandContext\<T\>

Namespace: MassTransit.Contracts

```csharp
public interface CommandContext<T> : CommandContext, PipeContext
```

#### Type Parameters

`T`<br/>

Implements [CommandContext](../masstransit-contracts/commandcontext), [PipeContext](../masstransit/pipecontext)

## Properties

### **Command**

The command object

```csharp
public abstract T Command { get; }
```

#### Property Value

T<br/>
