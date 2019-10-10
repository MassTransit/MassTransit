namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using Castle.MicroKernel;
    using Courier;
    using GreenPipes;
    using Scoping;
    using Scoping.CourierContexts;


    public class WindsorCompensateActivityScopeProvider<TActivity, TLog> :
        ICompensateActivityScopeProvider<TActivity, TLog>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IKernel _kernel;

        public WindsorCompensateActivityScopeProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ICompensateActivityScopeContext<TActivity, TLog> GetScope(CompensateContext<TLog> context)
        {
            if (context.TryGetPayload<IKernel>(out var kernel))
            {
                kernel.UpdateScope(context);

                var activity = kernel.Resolve<TActivity>(new Arguments().AddTyped(context.Log));

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);

                return new ExistingCompensateActivityScopeContext<TActivity, TLog>(activityContext, ReleaseComponent);
            }

            var scope = _kernel.CreateNewOrUseExistingMessageScope(context);
            try
            {
                var activity = _kernel.Resolve<TActivity>(new Arguments().AddTyped(context.Log));

                CompensateActivityContext<TActivity, TLog> activityContext = context.CreateActivityContext(activity);
                activityContext.UpdatePayload(_kernel);

                return new CreatedCompensateActivityScopeContext<IDisposable, TActivity, TLog>(scope, activityContext, ReleaseComponent);
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
