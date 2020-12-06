# Custom

Middleware components are configured using extension methods, to make them easy to discover.

::: tip NOTE
By default, a middleware configuration method should start with `Use`.
:::

An example middleware component that would log exceptions to the console is shown below.

```csharp
Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.UseExceptionLogger();
});
```

The extension method creates the pipe specification for the middleware component, which can be added to any pipe. For a component on the message consumption pipeline, use `ConsumeContext` instead of any `PipeContext`.

```csharp
public static class ExampleMiddlewareConfiguratorExtensions
{
    public static void UseExceptionLogger<T>(this IPipeConfigurator<T> configurator)
        where T : class, PipeContext
    {
        configurator.AddPipeSpecification(new ExceptionLoggerSpecification<T>());
    }
}
```

The pipe specification is a class that adds the filter to the pipeline. Additional logic can be included, such as configuring optional settings, etc. using a closure syntax similar to the other configuration classes in MassTransit.

```csharp
public class ExceptionLoggerSpecification<T> :
    IPipeSpecification<T>
    where T : class, PipeContext
{
    public IEnumerable<ValidationResult> Validate()
    {
        return Enumerable.Empty<ValidationResult>();
    }

    public void Apply(IPipeBuilder<T> builder)
    {
        builder.AddFilter(new ExceptionLoggerFilter<T>());
    }
}
```

Finally, the middleware component itself is a filter added to the pipeline. All filters have absolute and complete control of the execution context and flow of the message. Pipelines are entirely asynchronous, and expect that asynchronous operations will be performed.

::: danger
Do not use legacy constructs such as .Wait, .Result, or .WaitAll() as these can cause blocking in the message pipeline. While they might work in same cases, you've been warned!
:::


```csharp
public class ExceptionLoggerFilter<T> :
    IFilter<T>
    where T : class, PipeContext
{
    long _exceptionCount;
    long _successCount;
    long _attemptCount;

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateFilterScope("exceptionLogger");
        scope.Add("attempted", _attemptCount);
        scope.Add("succeeded", _successCount);
        scope.Add("faulted", _exceptionCount);
    }

    /// <summary>
    /// Send is called for each context that is sent through the pipeline
    /// </summary>
    /// <param name="context">The context sent through the pipeline</param>
    /// <param name="next">The next filter in the pipe, must be called or the pipe ends here</param>
    public async Task Send(T context, IPipe<T> next)
    {
        try
        {
            Interlocked.Increment(ref _attemptCount);

            // here the next filter in the pipe is called
            await next.Send(context);

            Interlocked.Increment(ref _successCount);
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _exceptionCount);

            await Console.Out.WriteLineAsync($"An exception occurred: {ex.Message}");

            // propagate the exception up the call stack
            throw;
        }
    }
}
```

The example filter above is stateful. If the filter was stateless, the same filter instance could be used by multiple pipes — worth considering if the filter has high memory requirements.

### Message Type Filters

In many cases, the message type is used by a filter. To create an instance of a generic filter that includes the message type, use the configuration observer. 

```cs
public class MessageFilterConfigurationObserver :
    ConfigurationObserver,
    IMessageConfigurationObserver
{
    public MessageFilterConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator)
        : base(receiveEndpointConfigurator)
    {
        Connect(this);
    }

    public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
        where TMessage : class
    {
        var specification = new MessageFilterPipeSpecification<TMessage>();

        configurator.AddPipeSpecification(specification);
    }
}
```

Then, in the specification, the appropriate filter can be created and added to the pipeline.

```cs
public class MessageFilterPipeSpecification<T> :
    IPipeSpecification<ConsumeContext<T>>
    where T : class
{
    public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
    {
        var filter = new MessageFilter<T>();

        builder.AddFilter(filter);
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield break;
    }
}
```

The filter could then include the message type as a generic type parameter.

```cs
public class MessageFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {        
        var scope = context.CreateFilterScope("messageFilter");
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        // do something

        await next.Send(context);
    }
}
```

The extension method for the above is shown below (for completeness).

```cs
public static class MessageFilterConfigurationExtensions
{
    public static void UseMessageFilter(this IConsumePipeConfigurator configurator)
    {
        if (configurator == null)
            throw new ArgumentNullException(nameof(configurator));

        var observer = new MessageFilterConfigurationObserver(configurator);
    }
}
```
