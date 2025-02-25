namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Collections.Concurrent;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.Util;
    using Configuration;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    public class ActiveMqConnectionContext :
        BasePipeContext,
        ConnectionContext,
        IAsyncDisposable
    {
        readonly IConnection _connection;
        readonly TaskExecutor _executor;
        readonly ConcurrentDictionary<string, IDestination> _temporaryEntities;

        /// <summary>
        /// Regular expression to distinguish if a destination is not for consuming data from a VirtualTopic. If yes we must get a standard destination because the name of
        /// the destination must match specific
        /// pattern. A temporary destination has generated name.
        /// </summary>
        /// <seealso href="https://activemq.apache.org/virtual-destinations">Virtual Destinations</seealso>
        readonly Regex _virtualTopicConsumerPattern;

        public ActiveMqConnectionContext(IConnection connection, IActiveMqHostConfiguration hostConfiguration, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _connection = connection;

            Description = hostConfiguration.Settings.ToDescription();
            HostAddress = hostConfiguration.HostAddress;

            Topology = hostConfiguration.Topology;

            _executor = new TaskExecutor();
            _temporaryEntities = new ConcurrentDictionary<string, IDestination>();

            _virtualTopicConsumerPattern = new Regex(hostConfiguration.Topology.PublishTopology.VirtualTopicConsumerPattern, RegexOptions.Compiled);
        }

        public IConnection Connection => _connection;
        public string Description { get; }
        public Uri HostAddress { get; }
        public IActiveMqBusTopology Topology { get; }

        public async Task<ISession> CreateSession(CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return await _executor.Run(() => _connection.CreateSessionAsync(AcknowledgementMode.IndividualAcknowledge), tokenSource.Token)
                .ConfigureAwait(false);
        }

        public bool IsVirtualTopicConsumer(string name)
        {
            return _virtualTopicConsumerPattern.IsMatch(name);
        }

        public IQueue GetTemporaryQueue(ISession session, string topicName)
        {
            return (IQueue)_temporaryEntities.GetOrAdd(topicName, _ => (IQueue)SessionUtil.GetDestination(session, topicName, DestinationType.TemporaryQueue));
        }

        public ITopic GetTemporaryTopic(ISession session, string topicName)
        {
            return (ITopic)_temporaryEntities.GetOrAdd(topicName, _ => (ITopic)SessionUtil.GetDestination(session, topicName, DestinationType.TemporaryTopic));
        }

        public bool TryGetTemporaryEntity(string name, out IDestination destination)
        {
            return _temporaryEntities.TryGetValue(name, out destination);
        }

        public bool TryRemoveTemporaryEntity(ISession session, string name)
        {
            if (_temporaryEntities.TryGetValue(name, out var destination))
            {
                session.DeleteDestination(destination);
                return true;
            }

            return false;
        }

        public async ValueTask DisposeAsync()
        {
            TransportLogMessages.DisconnectHost(Description);

            try
            {
                await _connection.CloseAsync().ConfigureAwait(false);

                TransportLogMessages.DisconnectedHost(Description);

                _connection.Dispose();

                await _executor.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Close Connection Faulted: {Host}", Description);
            }
        }
    }
}
