---

title: IHandlerConfigurationObserver

---

# IHandlerConfigurationObserver

Namespace: MassTransit

```csharp
public interface IHandlerConfigurationObserver
```

## Methods

### **HandlerConfigured\<TMessage\>(IHandlerConfigurator\<TMessage\>)**

Called when a consumer/message combination is configured

```csharp
void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../masstransit/ihandlerconfigurator-1)<br/>
