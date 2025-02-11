---

title: Behavior

---

# Behavior

Namespace: MassTransit.SagaStateMachine

A behavior is invoked by a state when an event is raised on the instance and embodies
 the activities that are executed in response to the event.

```csharp
public static class Behavior
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Behavior](../masstransit-sagastatemachine/behavior)

## Methods

### **Empty\<TSaga\>()**

Returns an empty pipe of the specified context type

```csharp
public static IBehavior<TSaga> Empty<TSaga>()
```

#### Type Parameters

`TSaga`<br/>
The context type

#### Returns

[IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

### **Empty\<TSaga, TMessage\>()**

```csharp
public static IBehavior<TSaga, TMessage> Empty<TSaga, TMessage>()
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Returns

[IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>

### **Faulted\<TSaga\>()**

```csharp
public static IBehavior<TSaga> Faulted<TSaga>()
```

#### Type Parameters

`TSaga`<br/>

#### Returns

[IBehavior\<TSaga\>](../../masstransit-abstractions/masstransit/ibehavior-1)<br/>

### **Faulted\<TSaga, TMessage\>()**

```csharp
public static IBehavior<TSaga, TMessage> Faulted<TSaga, TMessage>()
```

#### Type Parameters

`TSaga`<br/>

`TMessage`<br/>

#### Returns

[IBehavior\<TSaga, TMessage\>](../../masstransit-abstractions/masstransit/ibehavior-2)<br/>
