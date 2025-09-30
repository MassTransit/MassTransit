namespace MassTransit.AmazonSqsTransport;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS.Model;
using MassTransit.Middleware;
using Topology;


public class ScopeClientContext :
    ScopePipeContext,
    ClientContext,
    IDisposable
{
    readonly CancellationToken _cancellationToken;
    readonly ClientContext _context;
    CancellationTokenSource? _tokenSource;

    public ScopeClientContext(ClientContext context, CancellationToken cancellationToken)
        : base(context)
    {
        _context = context;

        _cancellationToken = cancellationToken;
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, cancellationToken);
    }

    public override CancellationToken CancellationToken => _tokenSource?.Token ?? _cancellationToken;

    public ConnectionContext ConnectionContext => _context.ConnectionContext;

    public async Task<TopicInfo> CreateTopic(Topology.Topic topic, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.CreateTopic(topic, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<QueueInfo> CreateQueue(Queue queue, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.CreateQueue(queue, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<bool> CreateQueueSubscription(Topology.Topic topic, Queue queue, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.CreateQueueSubscription(topic, queue, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task DeleteTopic(Topology.Topic topic, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.DeleteTopic(topic, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task DeleteQueue(Queue queue, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.DeleteQueue(queue, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task Publish(string topicName, PublishBatchRequestEntry request, CancellationToken cancellationToken = default)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.Publish(topicName, request, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task SendMessage(string queueName, SendMessageBatchRequestEntry request, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.SendMessage(queueName, request, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task DeleteMessage(string queueUrl, string receiptHandle, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.DeleteMessage(queueUrl, receiptHandle, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task PurgeQueue(string queueName, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.PurgeQueue(queueName, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<IList<Message>> ReceiveMessages(string queueName, int messageLimit, int waitTime, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.ReceiveMessages(queueName, messageLimit, waitTime, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task<QueueInfo> GetQueueInfo(string queueName, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        return await _context.GetQueueInfo(queueName, tokenSource.Token).ConfigureAwait(false);
    }

    public async Task ChangeMessageVisibility(string queueUrl, string receiptHandle, int seconds, CancellationToken cancellationToken)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

        await _context.ChangeMessageVisibility(queueUrl, receiptHandle, seconds, tokenSource.Token).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _tokenSource?.Dispose();
        _tokenSource = null;
    }
}
