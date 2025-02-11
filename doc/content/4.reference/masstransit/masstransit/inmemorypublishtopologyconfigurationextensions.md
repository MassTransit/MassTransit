---

title: InMemoryPublishTopologyConfigurationExtensions

---

# InMemoryPublishTopologyConfigurationExtensions

Namespace: MassTransit

```csharp
public static class InMemoryPublishTopologyConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InMemoryPublishTopologyConfigurationExtensions](../masstransit/inmemorypublishtopologyconfigurationextensions)

## Methods

### **AddPublishMessageTypesFromNamespaceContaining\<T\>(IInMemoryBusFactoryConfigurator, Action\<IInMemoryMessagePublishTopologyConfigurator, Type\>, Func\<Type, Boolean\>)**

Adds any valid message types found in the specified namespace to the publish topology

```csharp
public static void AddPublishMessageTypesFromNamespaceContaining<T>(IInMemoryBusFactoryConfigurator configurator, Action<IInMemoryMessagePublishTopologyConfigurator, Type> configure, Func<Type, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IInMemoryBusFactoryConfigurator](../masstransit/iinmemorybusfactoryconfigurator)<br/>

`configure` [Action\<IInMemoryMessagePublishTopologyConfigurator, Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddPublishMessageTypesFromNamespaceContaining(IInMemoryBusFactoryConfigurator, Type, Action\<IInMemoryMessagePublishTopologyConfigurator, Type\>, Func\<Type, Boolean\>)**

Adds any valid message types found in the specified namespace to the publish topology

```csharp
public static void AddPublishMessageTypesFromNamespaceContaining(IInMemoryBusFactoryConfigurator configurator, Type type, Action<IInMemoryMessagePublishTopologyConfigurator, Type> configure, Func<Type, bool> filter)
```

#### Parameters

`configurator` [IInMemoryBusFactoryConfigurator](../masstransit/iinmemorybusfactoryconfigurator)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to use to identify the assembly and namespace to scan

`configure` [Action\<IInMemoryMessagePublishTopologyConfigurator, Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddPublishMessageTypes(IInMemoryBusFactoryConfigurator, IEnumerable\<Type\>, Action\<IInMemoryMessagePublishTopologyConfigurator, Type\>)**

Adds the specified message types to the publish topology

```csharp
public static void AddPublishMessageTypes(IInMemoryBusFactoryConfigurator configurator, IEnumerable<Type> messageTypes, Action<IInMemoryMessagePublishTopologyConfigurator, Type> configure)
```

#### Parameters

`configurator` [IInMemoryBusFactoryConfigurator](../masstransit/iinmemorybusfactoryconfigurator)<br/>

`messageTypes` [IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`configure` [Action\<IInMemoryMessagePublishTopologyConfigurator, Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
