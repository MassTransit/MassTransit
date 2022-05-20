#nullable enable
namespace MassTransit.Logging
{
    using Courier.Contracts;
    using Microsoft.Extensions.Logging;
    using Transports;


    /// <summary>
    /// Used to provide access to logging and diagnostic services
    /// </summary>
    public interface ILogContext
    {
        /// <summary>
        /// The log context for all message movement, sent, received, etc.
        /// </summary>
        ILogContext Messages { get; }

        ILogger Logger { get; }
        EnabledLogger? Critical { get; }
        EnabledLogger? Debug { get; }
        EnabledLogger? Error { get; }
        EnabledLogger? Info { get; }
        EnabledLogger? Trace { get; }
        EnabledLogger? Warning { get; }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.Logging.ILogger" />.</returns>
        ILogContext CreateLogContext(string categoryName);

        StartedActivity? StartSendActivity<T>(SendTransportContext transportContext, SendContext<T> context, params (string Key, object Value)[] tags)
            where T : class;

        StartedActivity? StartReceiveActivity(string name, string inputAddress, string endpointName, ReceiveContext context,
            params (string Key, string Value)[] tags);

        StartedActivity? StartConsumerActivity<TConsumer, T>(ConsumeContext<T> context)
            where T : class;

        StartedActivity? StartHandlerActivity<T>(ConsumeContext<T> context)
            where T : class;

        StartedActivity? StartSagaActivity<TSaga, T>(SagaConsumeContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class;

        StartedActivity? StartSagaStateMachineActivity<TSaga, T>(BehaviorContext<TSaga, T> context)
            where TSaga : class, ISaga
            where T : class;

        StartedActivity? StartExecuteActivity<TActivity, TArguments>(ConsumeContext<RoutingSlip> context)
            where TActivity : IExecuteActivity<TArguments>
            where TArguments : class;

        StartedActivity? StartCompensateActivity<TActivity, TLog>(ConsumeContext<RoutingSlip> context)
            where TActivity : ICompensateActivity<TLog>
            where TLog : class;
    }
}
