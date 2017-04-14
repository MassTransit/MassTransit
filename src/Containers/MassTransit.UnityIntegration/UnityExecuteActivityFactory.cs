using GreenPipes;
using MassTransit.Courier;
using MassTransit.Courier.Hosts;
using MassTransit.Logging;
using MassTransit.Util;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;

namespace MassTransit.UnityIntegration
{
    public class UnityExecuteActivityFactory<TActivity, TArguments> :
         ExecuteActivityFactory<TActivity, TArguments>
         where TActivity : class, ExecuteActivity<TArguments>
         where TArguments : class
    {
        static readonly ILog _log = Logger.Get<UnityExecuteActivityFactory<TActivity, TArguments>>();
        readonly IUnityContainer _container;

        public UnityExecuteActivityFactory(IUnityContainer container)
        {
            _container = container;
        }

        public async Task<ResultContext<ExecutionResult>> Execute(ExecuteContext<TArguments> context,
            IRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult> next)
        {
            using (var childContainer = _container.CreateChildContainer())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("ExecuteActivityFactory: Executing: {0}", TypeMetadataCache<TActivity>.ShortName);

                childContainer.RegisterInstance(typeof(ExecuteContext), context);


                var activity = childContainer.Resolve<TActivity>();


                ExecuteActivityContext<TActivity, TArguments> activityContext = new HostExecuteActivityContext<TActivity, TArguments>(activity, context);

                var consumerLifetimeScope = childContainer;
                activityContext.GetOrAddPayload(() => consumerLifetimeScope);

                return await next.Send(activityContext).ConfigureAwait(false);
            }
        }


    }


}
