namespace MassTransit.RabbitMqTransport;

using System;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using MassTransit.Middleware;
using RabbitMQ.Client;


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

    public IConnection Connection => _context.Connection;
    public string Description => _context.Description;
    public Uri HostAddress => _context.HostAddress;
    public bool PublisherConfirmation => _context.PublisherConfirmation;
    public BatchSettings BatchSettings => _context.BatchSettings;
    public TimeSpan ContinuationTimeout => _context.ContinuationTimeout;

    public TimeSpan StopTimeout => _context.StopTimeout;
    public IRabbitMqBusTopology Topology => _context.Topology;

    public async Task<IChannel> CreateChannel(ushort? concurrentMessageLimit, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.CreateChannel(concurrentMessageLimit, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<ChannelContext> CreateChannelContext(IAgent agent, ushort? concurrentMessageLimit, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.CreateChannelContext(agent, concurrentMessageLimit, tokenSource.Token).ConfigureAwait(false);
    }
}
