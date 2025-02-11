---

title: IRequestStateMachineMissingInstanceConfigurator

---

# IRequestStateMachineMissingInstanceConfigurator

Namespace: MassTransit

```csharp
public interface IRequestStateMachineMissingInstanceConfigurator
```

## Methods

### **Apply\<TInstance, TMessage\>(IMissingInstanceConfigurator\<TInstance, TMessage\>)**

```csharp
IPipe<ConsumeContext<TMessage>> Apply<TInstance, TMessage>(IMissingInstanceConfigurator<TInstance, TMessage> configurator)
```

#### Type Parameters

`TInstance`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IMissingInstanceConfigurator\<TInstance, TMessage\>](../../masstransit-abstractions/masstransit/imissinginstanceconfigurator-2)<br/>

#### Returns

[IPipe\<ConsumeContext\<TMessage\>\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
