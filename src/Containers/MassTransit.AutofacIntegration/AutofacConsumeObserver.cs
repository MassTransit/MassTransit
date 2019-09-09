namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using GreenPipes;


    public class AutofacConsumeObserver :
        IConsumeObserver
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacConsumeObserver()
        {
        }

        public AutofacConsumeObserver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeObserver>();

            return observer.PreConsume(context);
        }

        Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeObserver>();

            return observer.PostConsume(context);
        }

        Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeObserver>();

            return observer.ConsumeFault(context, exception);
        }

        ILifetimeScope GetLifetimeScope(PipeContext context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out var payload))
                return payload;

            return _lifetimeScope
                ?? throw new MassTransitException("The lifetime scope was not in the payload, and a default lifetime scope was not specified.");
        }
    }
}
