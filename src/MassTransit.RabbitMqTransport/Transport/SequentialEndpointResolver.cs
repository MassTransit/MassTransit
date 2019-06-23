namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Context;
    using RabbitMQ.Client;


    /// <summary>
    /// Creates an IHostnameSelector which sequentially chooses the next host name from the provided list based on index
    /// </summary>
    public class SequentialEndpointResolver :
        IRabbitMqEndpointResolver
    {
        readonly string[] _hostNames;
        string _lastHost;
        int _nextHostIndex;

        public SequentialEndpointResolver(string[] hostNames)
        {
            if (hostNames == null)
                throw new ArgumentNullException(nameof(hostNames));
            if (hostNames.Length == 0)
                throw new ArgumentException("At least one host name must be specified", nameof(hostNames));
            if (hostNames.All(string.IsNullOrWhiteSpace))
                throw new ArgumentException("At least one non-blank host name must be specified", nameof(hostNames));

            _hostNames = hostNames;
            _nextHostIndex = 0;
            _lastHost = "";
        }

        public string LastHost => _lastHost;

        public IEnumerable<AmqpTcpEndpoint> All()
        {
            do
            {
                _lastHost = _hostNames[_nextHostIndex % _hostNames.Length];
            }
            while (string.IsNullOrWhiteSpace(_lastHost));

            LogContext.Debug?.Log("Returning next host: {Host}", _lastHost);

            Interlocked.Increment(ref _nextHostIndex);

            yield return new AmqpTcpEndpoint(_lastHost);
        }
    }
}
