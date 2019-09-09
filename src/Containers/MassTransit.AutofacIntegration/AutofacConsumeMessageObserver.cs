namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using GreenPipes;


    public class AutofacConsumeMessageObserver<T> :
        IConsumeMessageObserver<T>
        where T : class
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacConsumeMessageObserver()
        {
        }

        public AutofacConsumeMessageObserver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        Task IConsumeMessageObserver<T>.PreConsume(ConsumeContext<T> context)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeMessageObserver<T>>();

            return observer.PreConsume(context);
        }

        Task IConsumeMessageObserver<T>.PostConsume(ConsumeContext<T> context)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeMessageObserver<T>>();

            return observer.PostConsume(context);
        }

        Task IConsumeMessageObserver<T>.ConsumeFault(ConsumeContext<T> context, Exception exception)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeMessageObserver<T>>();

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
