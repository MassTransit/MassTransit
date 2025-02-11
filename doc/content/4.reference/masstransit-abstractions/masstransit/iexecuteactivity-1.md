---

title: IExecuteActivity<TArguments>

---

# IExecuteActivity\<TArguments\>

Namespace: MassTransit

```csharp
public interface IExecuteActivity<TArguments> : IExecuteActivity
```

#### Type Parameters

`TArguments`<br/>

Implements [IExecuteActivity](../masstransit/iexecuteactivity)

## Methods

### **Execute(ExecuteContext\<TArguments\>)**

Execute the activity

```csharp
Task<ExecutionResult> Execute(ExecuteContext<TArguments> context)
```

#### Parameters

`context` [ExecuteContext\<TArguments\>](../masstransit/executecontext-1)<br/>
The execution context

#### Returns

[Task\<ExecutionResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
An execution result, created from the execution passed to the activity
