---

title: IMessageDataRepositorySelector

---

# IMessageDataRepositorySelector

Namespace: MassTransit.Configuration

Use one of the selector extension methods to create a  instance for the
 selected repository implementation.

```csharp
public interface IMessageDataRepositorySelector
```

## Properties

### **Configurator**

```csharp
public abstract IBusFactoryConfigurator Configurator { get; }
```

#### Property Value

[IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>
