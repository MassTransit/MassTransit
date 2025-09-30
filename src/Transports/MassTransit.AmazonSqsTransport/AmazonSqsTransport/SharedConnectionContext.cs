namespace MassTransit.AmazonSqsTransport;

using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Middleware;
using Topology;


public class SharedConnectionContext :
    ProxyPipeContext,
    ConnectionContext,
    IDisposable
{
    readonly CancellationToken _cancellationToken;
    readonly ConnectionContext _context;
    CancellationTokenSource? _tokenSource;

    public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
        : base(context)
    {
        _context = context;

        _cancellationToken = cancellationToken;
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, cancellationToken);
    }

    public override CancellationToken CancellationToken => _tokenSource?.Token ?? _cancellationToken;

    public IConnection Connection => _context.Connection;
    public Uri HostAddress => _context.HostAddress;
    public IAmazonSqsBusTopology Topology => _context.Topology;

    public async Task<QueueInfo> GetQueue(Queue queue, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.GetQueue(queue, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<QueueInfo> GetQueueByName(string name, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.GetQueueByName(name, tokenSource.Token).ConfigureAwait(false);
    }

    public Task<bool> RemoveQueueByName(string name)
    {
        return _context.RemoveQueueByName(name);
    }

    public async Task<TopicInfo> GetTopic(Topic topic, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.GetTopic(topic, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<TopicInfo> GetTopicByName(string name, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.GetTopicByName(name, tokenSource.Token).ConfigureAwait(false);
    }

    public Task<bool> RemoveTopicByName(string name)
    {
        return _context.RemoveTopicByName(name);
    }

    public ClientContext CreateClientContext(CancellationToken cancellationToken)
    {
        return _context.CreateClientContext(cancellationToken);
    }

    public void Dispose()
    {
        _tokenSource?.Dispose();
        _tokenSource = null;
    }
}
