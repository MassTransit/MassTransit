namespace MassTransit.GrpcTransport
{
    using System;
    using System.Collections.Generic;
    using Grpc.Core;


    public class ServerNodeContext :
        NodeContext
    {
        readonly ServerCallContext _context;

        public ServerNodeContext(ServerCallContext context, Uri nodeAddress, Guid sessionId, IReadOnlyDictionary<string, string> host)
        {
            _context = context;
            NodeAddress = nodeAddress;
            SessionId = sessionId;

            Host = new DictionaryHostInfo(host);
        }

        public NodeType NodeType => NodeType.Server;
        public Uri NodeAddress { get; }
        public Guid SessionId { get; }
        public HostInfo Host { get; set; }
    }
}
