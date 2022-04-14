#nullable enable
namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Observables;


    public interface SendTransportContext :
        PipeContext,
        ISendObserverConnector
    {
        /// <summary>
        /// The LogContext used for sending transport messages, to ensure proper activity filtering
        /// </summary>
        ILogContext LogContext { get; }

        string EntityName { get; }
        string ActivityName { get; }
        string ActivityDestination { get; }
        string ActivitySystem { get; }

        SendObservable SendObservers { get; }

        ISerialization Serialization { get; }

        /// <summary>
        /// Create the send context without the presence of a transport, but in a way that it can be used by the transport
        /// </summary>
        /// <param name="message"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class;
    }


    public interface SendTransportContext<TContext> :
        SendTransportContext,
        IPipeContextSource<TContext>
        where TContext : class, PipeContext
    {
        IEnumerable<IAgent> GetAgentHandles();

        /// <summary>
        /// Create the send
        /// </summary>
        /// <param name="context">The send transport context, which may be used to create the underlying send context</param>
        /// <param name="message">The message being sent</param>
        /// <param name="pipe">The developer supplied pipe to configure the send context</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        Task<SendContext<T>> CreateSendContext<T>(TContext context, T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class;

        Task Send<T>(TContext transportContext, SendContext<T> sendContext)
            where T : class;
    }
}
