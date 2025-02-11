---

title: IMessageConfigurationObserver

---

# IMessageConfigurationObserver

Namespace: MassTransit

```csharp
public interface IMessageConfigurationObserver
```

## Methods

### **MessageConfigured\<TMessage\>(IConsumePipeConfigurator)**

Called when a message pipeline is configured, for the very first time

```csharp
void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../masstransit/iconsumepipeconfigurator)<br/>
