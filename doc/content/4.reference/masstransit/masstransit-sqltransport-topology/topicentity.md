---

title: TopicEntity

---

# TopicEntity

Namespace: MassTransit.SqlTransport.Topology

```csharp
public class TopicEntity : Topic, TopicHandle, EntityHandle
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TopicEntity](../masstransit-sqltransport-topology/topicentity)<br/>
Implements [Topic](../masstransit-sqltransport-topology/topic), [TopicHandle](../masstransit-sqltransport-topology/topichandle), [EntityHandle](../masstransit-topology/entityhandle)

## Properties

### **NameComparer**

```csharp
public static IEqualityComparer<TopicEntity> NameComparer { get; }
```

#### Property Value

[IEqualityComparer\<TopicEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

### **EntityComparer**

```csharp
public static IEqualityComparer<TopicEntity> EntityComparer { get; }
```

#### Property Value

[IEqualityComparer\<TopicEntity\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1)<br/>

### **TopicName**

```csharp
public string TopicName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Id**

```csharp
public long Id { get; }
```

#### Property Value

[Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Topic**

```csharp
public Topic Topic { get; }
```

#### Property Value

[Topic](../masstransit-sqltransport-topology/topic)<br/>

## Constructors

### **TopicEntity(Int64, String)**

```csharp
public TopicEntity(long id, string name)
```

#### Parameters

`id` [Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64)<br/>

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
