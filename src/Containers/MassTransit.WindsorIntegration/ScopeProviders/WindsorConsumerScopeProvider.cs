namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using System.Collections.Generic;
    using Castle.MicroKernel;
    using Context;
    using GreenPipes;
    using Metadata;
    using Scoping;
    using Scoping.ConsumerContexts;


    public class WindsorConsumerScopeProvider :
        IConsumerScopeProvider
    {
        readonly IKernel _kernel;
        readonly IList<Action<ConsumeContext>> _scopeActions;

        public WindsorConsumerScopeProvider(IKernel kernel)
        {
            _kernel = kernel;
            _scopeActions = new List<Action<ConsumeContext>>();
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        public IConsumerScopeContext GetScope(ConsumeContext context)
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                return new ExistingConsumerScopeContext(context);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope(context);
            try
            {
                var proxy = new ConsumeContextScope(context, _kernel);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(proxy);

                return new CreatedConsumerScopeContext<IDisposable>(scope, proxy);
            }
            catch
            {
                scope?.Dispose();
                throw;
            }
        }

        public IConsumerScopeContext<TConsumer, T> GetScope<TConsumer, T>(ConsumeContext<T> context)
            where TConsumer : class
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                var consumer = kernel.Resolve<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer);

                return new ExistingConsumerScopeContext<TConsumer, T>(consumerContext, ReleaseComponent);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope(context);
            try
            {
                var consumer = _kernel.Resolve<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                var consumerContext = new ConsumerConsumeContextScope<TConsumer, T>(context, consumer, _kernel);

                foreach (Action<ConsumeContext> scopeAction in _scopeActions)
                    scopeAction(consumerContext);

                return new CreatedConsumerScopeContext<IDisposable, TConsumer, T>(scope, consumerContext, ReleaseComponent);
            }
            catch
            {
                scope?.Dispose();
                throw;
            }
        }

        public void AddScopeAction(Action<ConsumeContext> action)
        {
            _scopeActions.Add(action);
        }

        void ReleaseComponent<T>(T component)
        {
            _kernel.ReleaseComponent(component);
        }
    }
}
