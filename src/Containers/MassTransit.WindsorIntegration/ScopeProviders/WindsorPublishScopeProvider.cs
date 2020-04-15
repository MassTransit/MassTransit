namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using Castle.MicroKernel;
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.PublishContexts;


    public class WindsorPublishScopeProvider :
        IPublishScopeProvider
    {
        readonly IKernel _kernel;

        public WindsorPublishScopeProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        public IPublishScopeContext<T> GetScope<T>(PublishContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out _))
                return new ExistingPublishScopeContext<T>(context);

            var scope = _kernel.CreateNewMessageScope(context);
            try
            {
                var publishContext = new PublishContextScope<T>(context, _kernel);

                return new CreatedPublishScopeContext<IDisposable, T>(scope, publishContext);
            }
            catch
            {
                scope?.Dispose();
                throw;
            }
        }
    }
}
