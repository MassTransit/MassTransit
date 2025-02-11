---

title: MessageDataConfiguratorExtensions

---

# MessageDataConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class MessageDataConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MessageDataConfiguratorExtensions](../masstransit/messagedataconfiguratorextensions)

## Methods

### **UseMessageData(IBusFactoryConfigurator, IMessageDataRepository)**

Enable the loading of message data for the any message type that includes a MessageData property.

```csharp
public static void UseMessageData(IBusFactoryConfigurator configurator, IMessageDataRepository repository)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>

`repository` [IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>

### **UseMessageData(IBusFactoryConfigurator, Func\<IMessageDataRepositorySelector, IMessageDataRepository\>)**

Enable the loading of message data for the any message type that includes a MessageData property.

```csharp
public static IMessageDataRepository UseMessageData(IBusFactoryConfigurator configurator, Func<IMessageDataRepositorySelector, IMessageDataRepository> selector)
```

#### Parameters

`configurator` [IBusFactoryConfigurator](../../masstransit-abstractions/masstransit/ibusfactoryconfigurator)<br/>
The bus factory configurator.

`selector` [Func\<IMessageDataRepositorySelector, IMessageDataRepository\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The repository selector.
 See extension methods, e.g. [MessageDataRepositorySelectorExtensions.FileSystem(IMessageDataRepositorySelector, String)](../masstransit-messagedata-configuration/messagedatarepositoryselectorextensions#filesystemimessagedatarepositoryselector-string).

#### Returns

[IMessageDataRepository](../../masstransit-abstractions/masstransit/imessagedatarepository)<br/>
