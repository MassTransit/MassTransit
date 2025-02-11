---

title: IVisitable

---

# IVisitable

Namespace: MassTransit

Used to visit the state machine structure, so it can be displayed, etc.

```csharp
public interface IVisitable : IProbeSite
```

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Accept(StateMachineVisitor)**

A visitable site can accept the visitor and pass control to internal elements

```csharp
void Accept(StateMachineVisitor visitor)
```

#### Parameters

`visitor` [StateMachineVisitor](../masstransit/statemachinevisitor)<br/>
