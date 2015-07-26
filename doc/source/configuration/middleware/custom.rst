Adding your own Middleware
""""""""""""""""""""""""""

.. sourcecode:: csharp
    :linenos:

    Bus.Factory.CreateUsingInMemory(cfg =>
    {
        cfg.UseExample();

    });


Exposing your middleware for people to consume.

.. sourcecode:: csharp
    :linenos:

    public static class ExampleMiddlewareConfiguratorExtensions
    {
        public static void UseExample<T>(this IPipeConfigurator<T> configurator)
            where T : class, PipeContext
        {

            configurator.AddPipeSpecification(new ExampleMiddleware<T>());
        }
    }

The configuration class

.. sourcecode:: csharp
    :linenos:

    public class ExampleMiddleware<T> : IPipeSpecification<T>
        where T : class, PipeContext
    {
        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new ExampleFilter<T>());
        }
    }

The middleware filter

.. sourcecode:: csharp
    :linenos:

    public class ExampleFilter<T> : IFilter<T>
        where T : class, PipeContext
    {
        public void Probe(ProbeContext context)
        {
            var step = context.CreateFilterScope("example");
            step.Add("is-example", true);
        }

        public async Task Send(T context, IPipe<T> next)
        {
            try
            {
                //before actions
                await next.Send(context);
                //post actions
            }
            catch (Exception ex)
            {
                //error handling
                Console.WriteLine("abc");

                //if you want standard MT error handling
                throw;
            }
        }
    }

