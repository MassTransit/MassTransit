namespace MassTransit.AmazonSqsTransport;

using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Middleware;
using Topology;


public class SharedConnectionContext :
    ProxyPipeContext,
    ConnectionContext
{
    readonly ConnectionContext _context;

    public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
        : base(context)
    {
        _context = context;
        CancellationToken = cancellationToken;
    }

    public override CancellationToken CancellationToken { get; }

    IConnection ConnectionContext.Connection => _context.Connection;
    public Uri HostAddress => _context.HostAddress;
    public IAmazonSqsBusTopology Topology => _context.Topology;

    public Task<QueueInfo> GetQueue(Queue queue)
    {
        return _context.GetQueue(queue);
    }

    public Task<QueueInfo> GetQueueByName(string name)
    {
        return _context.GetQueueByName(name);
    }

    public Task<bool> RemoveQueueByName(string name)
    {
        return _context.RemoveQueueByName(name);
    }

    public Task<TopicInfo> GetTopic(Topic topic)
    {
        return _context.GetTopic(topic);
    }

    public Task<TopicInfo> GetTopicByName(string name)
    {
        return _context.GetTopicByName(name);
    }

    public Task<bool> RemoveTopicByName(string name)
    {
        return _context.RemoveTopicByName(name);
    }

    public ClientContext CreateClientContext(CancellationToken cancellationToken)
    {
        return _context.CreateClientContext(cancellationToken);
    }
}
