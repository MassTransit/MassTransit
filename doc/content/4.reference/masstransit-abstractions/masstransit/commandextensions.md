---

title: CommandExtensions

---

# CommandExtensions

Namespace: MassTransit

```csharp
public static class CommandExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CommandExtensions](../masstransit/commandextensions)

## Methods

### **SendCommand\<T\>(IPipe\<CommandContext\>, T)**

```csharp
public static Task SendCommand<T>(IPipe<CommandContext> pipe, T command)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<CommandContext\>](../masstransit/ipipe-1)<br/>

`command` T<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
