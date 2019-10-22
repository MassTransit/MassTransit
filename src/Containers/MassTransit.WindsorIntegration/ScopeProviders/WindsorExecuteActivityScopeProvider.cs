namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using Castle.MicroKernel;
    using Courier;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;


    public class WindsorExecuteActivityScopeProvider<TActivity, TArguments> :
        IExecuteActivityScopeProvider<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IKernel _kernel;

        public WindsorExecuteActivityScopeProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IExecuteActivityScopeContext<TActivity, TArguments> GetScope(ExecuteContext<TArguments> context)
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                var activity = kernel.Resolve<TActivity>(new Arguments().AddTyped(context.Arguments));

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);

                return new ExistingExecuteActivityScopeContext<TActivity, TArguments>(activityContext, ReleaseComponent);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope(context);
            try
            {
                var activity = _kernel.Resolve<TActivity>(new Arguments().AddTyped(context.Arguments));

                ExecuteActivityContext<TActivity, TArguments> activityContext = context.CreateActivityContext(activity);
                activityContext.UpdatePayload(_kernel);

                return new CreatedExecuteActivityScopeContext<IDisposable, TActivity, TArguments>(scope, activityContext, ReleaseComponent);
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        void ReleaseComponent<T>(T component)
        {
            _kernel.ReleaseComponent(component);
        }
    }
}
