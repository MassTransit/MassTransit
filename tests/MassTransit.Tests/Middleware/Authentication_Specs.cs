namespace MassTransit.Tests.Middleware
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using NUnit.Framework;


    public class Authentication_Specs
    {
        IPipe<RequestContextImpl> _thePipe;
        bool cleanUp;
        bool protectedBusinessAction;
        bool rejected;

        [SetUp]
        public void SetUp()
        {
            protectedBusinessAction = false;
            cleanUp = false;
            rejected = false;

            IPipe<RequestContextImpl> authPipe = Pipe.New<RequestContextImpl>(cfg =>
            {
                cfg.UseExecute(cxt =>
                {
                    protectedBusinessAction = true;
                });
            });

            IPipe<RequestContextImpl> unauthPipe = Pipe.New<RequestContextImpl>(cfg =>
            {
                cfg.UseExecute(cxt =>
                {
                    rejected = true;
                });
            });

            _thePipe = Pipe.New<RequestContextImpl>(cfg =>
            {
                cfg.UseAuthFilter(authPipe, unauthPipe, "bob");
                cfg.UseExecute(cxt =>
                {
                    cleanUp = true;
                });
            });
        }

        [Test]
        public async Task Authenticated()
        {
            var request = new RequestContextImpl();
            request.GetOrAddPayload(() => new GenericPrincipal(new GenericIdentity("Gizmo"), new[] { "bob" }));

            await _thePipe.Send(request).ConfigureAwait(false);

            Assert.That(protectedBusinessAction, Is.True);
            Assert.That(cleanUp, Is.True);
            Assert.That(rejected, Is.False);
        }

        [Test]
        public async Task InvalidSetup()
        {
            IPipe<RequestContextImpl> authPipe = Pipe.New<RequestContextImpl>(cfg =>
            {
                cfg.UseExecute(cxt =>
                {
                });
            });

            IPipe<RequestContextImpl> unauthPipe = Pipe.New<RequestContextImpl>(cfg =>
            {
            });

            Assert.That(() =>
            {
                Pipe.New<RequestContextImpl>(cfg =>
                {
                    cfg.UseAuthFilter(authPipe, unauthPipe);
                });
            }, Throws.TypeOf<ConfigurationException>());
        }
    }


    //A random context
    //Notice: No mention of IPrincipal any where
    public class RequestContextImpl :
        BasePipeContext,
        PipeContext
    {
    }


    //Play nice with CFG DSL using an extension method
    public static class AuthExtensions
    {
        public static void UseAuthFilter<T>(this IPipeConfigurator<T> cfg, IPipe<T> authPipe, IPipe<T> unauthPipe,
            params string[] allowedRoles)
            where T : class, PipeContext
        {
            cfg.AddPipeSpecification(new SampleAuthFilterSpecification<T>(authPipe, unauthPipe, allowedRoles));
        }
    }


    //Your custom filter specification with validation support
    public class SampleAuthFilterSpecification<T> : IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly string[] _allowedRoles;
        readonly IPipe<T> _authPipe;
        readonly IPipe<T> _unauthPipe;

        public SampleAuthFilterSpecification(IPipe<T> authPipe, IPipe<T> unauthPipe, string[] allowedRoles)
        {
            _authPipe = authPipe;
            _unauthPipe = unauthPipe;
            _allowedRoles = allowedRoles;
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new SampleAuthenticationFilter<T>(_authPipe, _unauthPipe, _allowedRoles));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_allowedRoles == null || !_allowedRoles.Any())
                yield return this.Failure("SampleAuthenticationFilter", "You need to supply roles");
        }
    }


    //your actual filter
    public class SampleAuthenticationFilter<T> : IFilter<T>
        where T : class, PipeContext
    {
        readonly string[] _allowedRoles;
        readonly IPipe<T> _authPipe;
        readonly IPipe<T> _unauthPipe;

        public SampleAuthenticationFilter(IPipe<T> authPipe, IPipe<T> unauthPipe, string[] allowedRoles)
        {
            _authPipe = authPipe;
            _unauthPipe = unauthPipe;
            _allowedRoles = allowedRoles;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("SampleAuthFilter");
            scope.Add("allowed-roles", _allowedRoles);

            _authPipe.Probe(scope.CreateScope("auth-pipe"));
            _unauthPipe.Probe(scope.CreateScope("unauth-pipe"));
        }

        public async Task Send(T context, IPipe<T> next)
        {
            IPrincipal prin;
            if (context.TryGetPayload(out prin))
            {
                if (_allowedRoles.Any(r => prin.IsInRole(r)))
                {
                    //you are allowed so continue to the next "segment"
                    await _authPipe.Send(context);
                }
                else
                    await _unauthPipe.Send(context);
            }

            await next.Send(context);
        }
    }
}
