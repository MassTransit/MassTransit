# Green Cache

Caching has long been a requirement for building fast systems, including microprocessors, file systems, databases, the internet, and even developer applications. Typically, trading a little memory for increased performance is a good thing - applications respond more quickly which leads to happy users.

MassTransit has tried a few different ways to cache things like send endpoints, to reduce the load on the broker and minimize application latency when sending messages. They've all sort of worked, but I really wanted to come up with a way to avoid duplicate work and also avoid cascade failures by caching failures. I also wanted to make sure highly dynamic systems that heavily use temporary queues don't run out of channels by keeping too many send endpoints cached that are only used once.

To that end, I created _Green Cache_.


## Cache Architecture

Green Cache is an in-memory, asynchronous, least-recently-used (LRU) cache optimized for indexed access to cached values. When using Green Cache, an index is used to read from the cache, with values added via a _missing value factory_ which can be provided to the `Get` operation. The cached values are held within the address of a single process, and are accessed directly by the code running in that process.

> This type of cache is very fast, requires no serialization, and is well suited for creating connection pools, session pools, and for maintaining handles to resources.

Green Cache is composed of a node tracker that keeps track of cached values which is then shared by the cache's indices. Each index has a key (that is strongly typed) which is a property on the cached value, that is the unique identity for the value.


## Asynchronous, for the, _await_ for it, win!

Cached values are read using an index, and that read operation is fully asynchronous. The `Get` method shown below has two key aspects that make it a really powerful abstraction for this type of cache.

```csharp
Task<TValue> Get(TKey key, MissingValueFactory<TKey, TValue> factory);
```

First the key, which should be obvious. If it were a `string`, a string would be provided to find the value using the index. The second argument is a factory method to create the value if it isn't found. This delegate is asynchronous as well, and is only called if the value is not found.

```csharp
public delegate Task<TValue> MissingValueFactory<in TKey, TValue>(TKey key);
```

The compositional nature of the TPL makes this a really strong abstraction -- the `Task` returned by the `Get` method above is not the same task that is returned from the missing value factory. In order to make cache access quick, the `Task` returned is a placeholder which is decoupled from the factory method. This placeholder reserves an index slot for the key specified, while the factory method is invoked asynchronously.

Adding the key to the index allows subsequent reads for the same key to receive the same placeholder as the first read -- preventing duplicate factory methods being executed. The factory methods for these reads aren't lost, however, and are stored by the placeholder in the order they were received. This approach makes it possible for dozens of tasks to wait on the creation of the value for the same key to be returned.

A subsequent reader for the same key may receive one of the following results:

* The value created by the first reader, if the factory method ran to completion.
* The value created by a subsequent reader that called `Get` before this reader.
* The value created by the factory method provided by this reader.
    - The value, if the factory method ran to completion
    - A faulted task, _only_ if the factory method provided by this reader throws an exception. This fault would only be visible to this reader, any subsequent readers would not see this exception.

If a subsequent reader does not provide a missing value factory, the reader will receive one of the following results:

* The value created by a previous reader, if any previous reader's factory method ran to completion.
* A `KeyNotFoundException` if no previous reader's factory method ran to completion (either cancelled or faulted).

Once the value has been created, the placeholder is replaced with a cached node by the node tracker. If the cache has multiple indices, the value is then propagated to the other indices, making it available to readers.

> An index by itself is consistent, you can read cached values and they'll exist once created, but there is no guarantee that another index will have the value until after it has been created and propagated.


## Recycling (staying Green)

> A cache without an eviction policy is a memory leak.

Green Cache uses two methods for managing memory usage, a capacity limit combined with an age limit.

The capacity limit specifies how many values can be stored in the cache. The cache capacity is dynamic and doesn't represent a fixed limit how many values are in the cache at a point in time. Instead, capacity works in combination with the minimum age to make the cache useful for short-lived values, while keeping the size of the cache under control long term.

The age limits define the minimum and maximum age of a value. The minimum age is a fixed lower limit specifying how long a value is cached, and a value will never be removed until it is of legal age. The maximum age is the longest an untouched value will remain in the cache before being removed.

> Yes, if you add one hundred million values per minute to a cache with a capacity of 1000, you will have one hundred _meeellllion_ values in your cache for a minute.


### Under the hood

To manage values, a node tracker determines the overall potential lifespan of a value (basically the difference between the maximum and minimum age) and splits that time period into buckets. The buckets are arranged in order by time, and accesses the buckets using a ring (which sounds really cool, but it's just using the `%` operator to avoid reusing a bucket index).

As time passes, the active range of buckets moves forward. Once the cache capacity has been reached, older buckets are emptied to make room for new values.

Values within a bucket can be touched, so that even if they are legally old enough to be removed from the cache they'll remain and be moved to the current bucket. This ensures that _lively_ values are kept in the cache longer than _cold fish_. 

The node tracker also have a maximum lifetime before it essentially _drops everything_ and restarts. The reason for this is to avoid weird things that happen when a cache lives forever and to allow the process memory structure a _reboot_ to avoid long-term garbage collection issues. The reason for this is simple -- years of building services that run 24x7 and seeing weird things magically fix themselves after restarting the service.


## Wrap up

That's about it for now. For MassTransit developers, this doesn't change anything on the surface as it is completely internal. I just found it interesting enough to share, both to get feedback and to come to some mental conclusion after the time spent creating _Green Cache_.


