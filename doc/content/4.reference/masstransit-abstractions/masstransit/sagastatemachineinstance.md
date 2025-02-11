---

title: SagaStateMachineInstance

---

# SagaStateMachineInstance

Namespace: MassTransit

An Automatonymous state machine instance that is usable as a saga by MassTransit must implement this interface.
 It indicates to the framework the available features of the state as being a state machine instance.

```csharp
public interface SagaStateMachineInstance : ISaga
```

Implements [ISaga](../masstransit/isaga)
