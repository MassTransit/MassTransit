---

title: ExceptionTypeCache

---

# ExceptionTypeCache

Namespace: MassTransit.SagaStateMachine

```csharp
public static class ExceptionTypeCache
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExceptionTypeCache](../masstransit-sagastatemachine/exceptiontypecache)

## Methods

### **Faulted\<TSaga\>(IBehavior\<TSaga\>, BehaviorContext\<TSaga\>, Exception)**

```csharp
public static Task Faulted<TSaga>(IBehavior<TSaga> behavior, BehaviorContext<TSaga> context, Exception exception)
```

#### Type Parameters

`TSaga`<br/>

#### Parameters

`behavior` [IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

`context` [BehaviorContext\<TSaga\>](../../masstransit-abstractions/masstransit/behaviorcontext-1)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **Faulted\<TSaga, TMessage\>(IBehavior\<TSaga, TMessage\>, BehaviorContext\<TSaga, TMessage\>, Exception)**

```csharp
public static Task Faulted<TSaga, TMessage>(IBehavior<TSaga, TMessage> behavior, BehaviorContext<TSaga, TMessage> context, Exception exception)
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Parameters

`behavior` [IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

`context` [BehaviorContext\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/behaviorcontext-2)<br/>

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
