---

title: SqlPublishTopologyConfigurationExtensions

---

# SqlPublishTopologyConfigurationExtensions

Namespace: MassTransit

```csharp
public static class SqlPublishTopologyConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SqlPublishTopologyConfigurationExtensions](../masstransit/sqlpublishtopologyconfigurationextensions)

## Methods

### **AddPublishMessageTypesFromNamespaceContaining\<T\>(ISqlBusFactoryConfigurator, Action\<ISqlMessagePublishTopologyConfigurator, Type\>, Func\<Type, Boolean\>)**

Adds any valid message types found in the specified namespace to the publish topology

```csharp
public static void AddPublishMessageTypesFromNamespaceContaining<T>(ISqlBusFactoryConfigurator configurator, Action<ISqlMessagePublishTopologyConfigurator, Type> configure, Func<Type, bool> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [ISqlBusFactoryConfigurator](../masstransit/isqlbusfactoryconfigurator)<br/>

`configure` [Action\<ISqlMessagePublishTopologyConfigurator, Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddPublishMessageTypesFromNamespaceContaining(ISqlBusFactoryConfigurator, Type, Action\<ISqlMessagePublishTopologyConfigurator, Type\>, Func\<Type, Boolean\>)**

Adds any valid message types found in the specified namespace to the publish topology

```csharp
public static void AddPublishMessageTypesFromNamespaceContaining(ISqlBusFactoryConfigurator configurator, Type type, Action<ISqlMessagePublishTopologyConfigurator, Type> configure, Func<Type, bool> filter)
```

#### Parameters

`configurator` [ISqlBusFactoryConfigurator](../masstransit/isqlbusfactoryconfigurator)<br/>

`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
The type to use to identify the assembly and namespace to scan

`configure` [Action\<ISqlMessagePublishTopologyConfigurator, Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>

`filter` [Func\<Type, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **AddPublishMessageTypes(ISqlBusFactoryConfigurator, IEnumerable\<Type\>, Action\<ISqlMessagePublishTopologyConfigurator, Type\>)**

Adds the specified message types to the publish topology

```csharp
public static void AddPublishMessageTypes(ISqlBusFactoryConfigurator configurator, IEnumerable<Type> messageTypes, Action<ISqlMessagePublishTopologyConfigurator, Type> configure)
```

#### Parameters

`configurator` [ISqlBusFactoryConfigurator](../masstransit/isqlbusfactoryconfigurator)<br/>

`messageTypes` [IEnumerable\<Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>

`configure` [Action\<ISqlMessagePublishTopologyConfigurator, Type\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-2)<br/>
