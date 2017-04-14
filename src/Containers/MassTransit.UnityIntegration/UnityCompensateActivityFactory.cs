namespace MassTransit.UnityIntegration
{
    public class UnityCompensateActivityFactory<TActivity, TLog> :
        CompensateActivityFactory<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        static readonly ILog _log = Logger.Get<UnityCompensateActivityFactory<TActivity, TLog>>();
        readonly IUnityContainer _container;

        public UnityCompensateActivityFactory(IUnityContainer container)
        {
            _container = container;
        }

        public async Task<ResultContext<CompensationResult>> Compensate(CompensateContext<TLog> context,
            IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> next)
        {
            using (var childContainer = _container.CreateChildContainer())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("CompensateActivityFactory: Compensating: {0}", TypeMetadataCache<TActivity>.ShortName);

                childContainer.RegisterInstance(typeof(CompensateContext), context);


                var activity = childContainer.Resolve<TActivity>();

                CompensateActivityContext<TActivity, TLog> activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                var consumerLifetimeScope = childContainer;
                activityContext.GetOrAddPayload(() => consumerLifetimeScope);

                return await next.Send(activityContext).ConfigureAwait(false);
            }
        }

    }

}
