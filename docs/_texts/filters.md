---
layout: default
title: Filters
subtitle: A unit of action in the pipeline
---

A filter in GreenPipes is a middleware component that performs a specific function. You can think of them as HTTP Modules in ASP.Net or the Filters in ASP.Net MVC. All of these types serve to do the same general thing, which is to provide a common service to all requests that pass through the pipeline. Because its a common service, each filter should strongly adhere to the [single responsibility principle](http://en.wikipedia.org/wiki/Single_responsibility_principle) -- do one thing and one thing only. This fine-grained approach ensures that developers are able to opt-in to each behavior without including unnecessary or unwatched functionality.

> The overhead of having many fine-grained filters is minimal as each filter does NOT have to do any casting of objects and amounts to nothing more fancy than a method call. The throughput of various filter stacks can be testing in the `gpbench` app that comes with the project if you would like to test any combination of filters.

## Included Filters

- Circuit Breaker
- Concurrency Limit
- Log
- Rate Limit
- Repeat
- Rescue
- Retry
- and more (the full list is located at https://github.com/phatboyg/GreenPipes/tree/master/src/GreenPipes/Filters)

## Writing your own filter

GreenPipes itself follows a fairly common pattern when creating a new filter. Below I will demonstrate writing a naive authorization filter. This filter will inspect the payload context and look for an IPrincipal, if it finds one, it will assert that the principal has one of the required roles. If it does, it will let the request pass, otherwise it will divert and send the request down an Unauthorized pipeline.

1. Create the Filter itself, which is a class that implements `IFilter<T>`.

```
public class AuthorizationFilter<T> : IFilter<T>
        where T : class, PipeContext
    {
        readonly IPipe<T> _unauthPipe;
        readonly string[] _allowedRoles;

        public AuthorizationFilter(IPipe<T> unauthPipe, string[] allowedRoles)
        {
            _unauthPipe = unauthPipe;
            _allowedRoles = allowedRoles;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("AuthorizationFilter");
            scope.Add("allowed-roles", _allowedRoles);

            _unauthPipe.Probe(scope.CreateScope("unauth-pipe"));
        }

        public async Task Send(T context, IPipe<T> next)
        {
            IPrincipal prin;
            if (context.TryGetPayload(out prin))
            {
                if (!_allowedRoles.Any(r => prin.IsInRole(r)))
                {
                    await _unauthPipe.Send(context);
                }
            }

            await next.Send(context);
        }
    }
```

2. Create the Filter Specification. This will allow you to create the instance of the filter using the configuration data as well as validate the filter usage.

```
public class AuthorizationFilterSpecification<T> : IPipeSpecification<T>
    where T : class, PipeContext
{
    readonly IPipe<T> _unauthPipe;
    readonly string[] _allowedRoles;

    public AuthorizationFilterSpecification(IPipe<T> unauthPipe, string[] allowedRoles)
    {
        _unauthPipe = unauthPipe;
        _allowedRoles = allowedRoles;
    }

    public void Apply(IPipeBuilder<T> builder)
    {
        builder.AddFilter(new AuthorizationFilter<T>(_unauthPipe, _allowedRoles));
    }

    public IEnumerable<ValidationResult> Validate()
    {
        if (_allowedRoles == null || !_allowedRoles.Any())
            yield return this.Failure("AuthorizationFilter", "You need to supply at least 1 role");
    }
}
```

3. Write a simple extension method to bring a nice DSL to using your filter by building the specification with a nice configuration syntax.

```
public static class AuthorizationFilterExtensions
{
    public static void UseAuthorizationFilter<T>(this IPipeConfigurator<T> cfg, IPipe<T> unauthPipe,
        params string[] allowedRoles)
        where T : class, PipeContext
    {
        cfg.AddPipeSpecification(new AuthorizationFilterSpecification<T>(unauthPipe, allowedRoles));
    }
}
```

4. Finally lets test it.

```
public class Authentication_Specs
{
   IPipe<RequestContext> _thePipe;
   bool protectedBusinessAction;
   bool rejected;

   [SetUp]
   public void SetUp()
   {
       protectedBusinessAction = false;
       rejected = false;

       var unauthPipe = Pipe.New<RequestContext>(cfg =>
       {
           cfg.UseExecute(cxt =>
           {
               rejected = true;
           });
       });

       _thePipe = Pipe.New<RequestContext>(cfg =>
       {
           //Unauthorized users will be diverted down the `unauthPipe`
           cfg.UseAuthFilter(unauthPipe, "bob");

           //If you are authorized you will be able to continue down the pipe
           cfg.UseExecute(cxt =>
           {
              protectedBusinessAction = true;
           });
       });
   }

   [TearDown]
   public void TearDown()
   {
       Console.WriteLine(_thePipe.GetProbeResult().ToJsonString());
   }

   [Test]
   public async Task Authenticated()
   {
       var request = new RequestContext();
       request.GetOrAddPayload(() => new GenericPrincipal(new GenericIdentity("Gizmo"), new []{"bob"} ));

       await _thePipe.Send(request).ConfigureAwait(false);

       Assert.That(protectedBusinessAction, Is.True);
       Assert.That(rejected, Is.False);
   }

   [Test]
   public async Task Unauthenticated()
   {
       var request = new RequestContext();
       request.GetOrAddPayload(() => new GenericPrincipal(new GenericIdentity("Gremlin"), new []{""} ));

       await _thePipe.Send(request).ConfigureAwait(false);

       Assert.That(protectedBusinessAction, Is.False);
       Assert.That(rejected, Is.True);
   }

   [Test]
   public async Task InvalidSetup()
   {
       var unauthPipe = Pipe.New<RequestContext>(cfg =>
       {

       });

       Assert.That(() =>
       {
           Pipe.New<RequestContext>(cfg =>
           {
               cfg.UseAuthFilter(authPipe, unauthPipe);
               cfg.UseExecute(cxt =>
               {
                   protectedBusinessAction = true;
               });
           });
       }, Throws.TypeOf<PipeConfigurationException>());

   }
}

//A random context
//Notice: No mention of IPrincipal any where
public class RequestContext :
   BasePipeContext,
   PipeContext
{
   public RequestContext() : base(new PayloadCache())
   {
   }

}
```
