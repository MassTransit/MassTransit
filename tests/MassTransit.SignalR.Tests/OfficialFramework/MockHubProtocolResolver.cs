namespace MassTransit.SignalR.Tests.OfficialFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;

    class MockHubProtocolResolver : IHubProtocolResolver
    {
        readonly Dictionary<string, IHubProtocol> _availableProtocols;
        readonly List<IHubProtocol> _hubProtocols;


        public MockHubProtocolResolver(IEnumerable<IHubProtocol> availableProtocols)
        {
            _availableProtocols = new Dictionary<string, IHubProtocol>(StringComparer.OrdinalIgnoreCase);

            foreach (var protocol in availableProtocols)
            {
                _availableProtocols[protocol.Name] = protocol;
            }

            _hubProtocols = _availableProtocols.Values.ToList();
        }

        public IHubProtocol GetProtocol(string protocolName, IReadOnlyList<string> supportedProtocols)
        {
            protocolName = protocolName ?? throw new ArgumentNullException(nameof(protocolName));

            if (_availableProtocols.TryGetValue(protocolName, out var protocol) && (supportedProtocols == null || supportedProtocols.Contains(protocolName, StringComparer.OrdinalIgnoreCase)))
            {
                return protocol;
            }

            // null result indicates protocol is not supported
            // result will be validated by the caller
            return null;
        }

        public IReadOnlyList<IHubProtocol> AllProtocols => _hubProtocols;
    }
}
