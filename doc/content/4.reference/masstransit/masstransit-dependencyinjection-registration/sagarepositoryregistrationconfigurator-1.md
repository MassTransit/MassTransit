---

title: SagaRepositoryRegistrationConfigurator<TSaga>

---

# SagaRepositoryRegistrationConfigurator\<TSaga\>

Namespace: MassTransit.DependencyInjection.Registration

```csharp
public class SagaRepositoryRegistrationConfigurator<TSaga> : ISagaRepositoryRegistrationConfigurator<TSaga>, IServiceCollection, IList<ServiceDescriptor>, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable
```

#### Type Parameters

`TSaga`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaRepositoryRegistrationConfigurator\<TSaga\>](../masstransit-dependencyinjection-registration/sagarepositoryregistrationconfigurator-1)<br/>
Implements [ISagaRepositoryRegistrationConfigurator\<TSaga\>](../masstransit/isagarepositoryregistrationconfigurator-1), IServiceCollection, [IList\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1), [ICollection\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1), [IEnumerable\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Count**

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **IsReadOnly**

```csharp
public bool IsReadOnly { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Item**

```csharp
public ServiceDescriptor Item { get; set; }
```

#### Property Value

ServiceDescriptor<br/>

## Constructors

### **SagaRepositoryRegistrationConfigurator(IServiceCollection)**

```csharp
public SagaRepositoryRegistrationConfigurator(IServiceCollection collection)
```

#### Parameters

`collection` IServiceCollection<br/>

## Methods

### **GetEnumerator()**

```csharp
public IEnumerator<ServiceDescriptor> GetEnumerator()
```

#### Returns

[IEnumerator\<ServiceDescriptor\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br/>

### **Add(ServiceDescriptor)**

```csharp
public void Add(ServiceDescriptor item)
```

#### Parameters

`item` ServiceDescriptor<br/>

### **Clear()**

```csharp
public void Clear()
```

### **Contains(ServiceDescriptor)**

```csharp
public bool Contains(ServiceDescriptor item)
```

#### Parameters

`item` ServiceDescriptor<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CopyTo(ServiceDescriptor[], Int32)**

```csharp
public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
```

#### Parameters

`array` ServiceDescriptor[]<br/>

`arrayIndex` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Remove(ServiceDescriptor)**

```csharp
public bool Remove(ServiceDescriptor item)
```

#### Parameters

`item` ServiceDescriptor<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IndexOf(ServiceDescriptor)**

```csharp
public int IndexOf(ServiceDescriptor item)
```

#### Parameters

`item` ServiceDescriptor<br/>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Insert(Int32, ServiceDescriptor)**

```csharp
public void Insert(int index, ServiceDescriptor item)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`item` ServiceDescriptor<br/>

### **RemoveAt(Int32)**

```csharp
public void RemoveAt(int index)
```

#### Parameters

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
