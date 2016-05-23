namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
    using RabbitMQ.Client;

    /// <summary>
    /// Creates an IHostnameSelector which sequentially chooses the next host name from the provided list based on index
    /// </summary>
    public class RabbitMqSequentialHostnameSelector : IHostnameSelector
    {
        static readonly ILog _log = Logger.Get<RabbitMqSequentialHostnameSelector>();

        private int _nextHostIndex;

        public RabbitMqSequentialHostnameSelector()
        {
            _nextHostIndex = 0;
        }

        public string NextFrom(IList<string> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (!options.Any())
            {
                throw new ArgumentException("There must be at least one host to use a hostname selector.", "options");
            }

            //Clamp host index
            if (_nextHostIndex >= options.Count())
            {
                _nextHostIndex = 0;
            }

            string host = options[_nextHostIndex];
            if (_log.IsDebugEnabled)
            {
                _log.Debug($"Using new hostname from pool; {host}");
            }
            _nextHostIndex++;
            return host;
        }
    }
}
