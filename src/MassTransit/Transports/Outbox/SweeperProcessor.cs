using GreenPipes;
using MassTransit.Serialization;
using MassTransit.Transports.Outbox.Configuration;
using MassTransit.Transports.Outbox.Entities;
using MassTransit.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class SweeperProcessor : ISweeperProcessor
    {
        private readonly OutboxInstanceState _state;
        private readonly IBus _bus;
        private readonly ILogger _logger;
        private readonly IOutboxSemaphore _semaphore;
        private readonly IOutboxTransportOptions _outboxTransportOptions;
        private readonly ISweeperRepository _repository;

        public SweeperProcessor(
            OutboxInstanceState state,
            IBus bus,
            ILogger<SweeperProcessor> logger,
            IOutboxSemaphore semaphore,
            IOutboxTransportOptions options,
            ISweeperRepository repository)
        {
            _state = state;
            _bus = bus;
            _logger = logger;
            _semaphore = semaphore;
            _outboxTransportOptions = options;
            _repository = repository;
        }

        public async Task ExecuteAsync(Guid requestorId, CancellationToken cancellationToken)
        {
            var messages = await FetchMessages(requestorId, cancellationToken);
            while (!cancellationToken.IsCancellationRequested && messages.Any())
            {
                List<Task<OutboxMessage>> sendMessages = messages.Select(x => SendMessage(x, cancellationToken)).ToList();

                var failedMessages = new List<OutboxMessage>();

                foreach (var bucket in TaskUtil.Interleaved(sendMessages))
                {
                    var finishedTask = await bucket.ConfigureAwait(false);

                    try
                    {
                        var message = await finishedTask.ConfigureAwait(false);

                        if(!_outboxTransportOptions.BulkRemove)
                        {
                            // This trickles the updates into the DB as they finish
                            // Less likely to have duplicates in the event of a transport failure, but slower
                            await _repository.RemoveMessage(message, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        // Swallow
                        _logger.LogWarning(e, "SendMessage attempt was cancelled, releasing message");
                    }
                    catch (OutboxSweeperSendException e)
                    {
                        // Swallow
                        _logger.LogError(e, "Failed to send message, incrementing retries count");

                        try
                        {
                            if(_outboxTransportOptions.BulkRemove)
                                failedMessages.Add(e.OutboxMessage);
                            else
                                await _repository.FailedToSendMessage(e).ConfigureAwait(false);
                        }
                        catch (Exception ee)
                        {
                            _logger.LogError(ee, $"Failed to increment retries count on message '{e.OutboxMessage.OutboxName}'+'{e.OutboxMessage.Id}'");
                        }
                    }
                    catch (Exception e)
                    {
                        // All other failures here. All we can do is Log it.
                        _logger.LogError(e, "Sweeper Failed to send message and received an unknown reason.");
                    }
                }

                if(_outboxTransportOptions.BulkRemove)
                {
                    if(failedMessages.Count == 0)
                    {
                        await _repository.RemoveAllMessages(_outboxTransportOptions.OutboxName, _state.InstanceId, cancellationToken);
                    }
                    else
                    {
                        await _repository.RemoveAllCompletedMessages(messages.Except(failedMessages, (x, y) => x.Id == y.Id).ToList(), cancellationToken);

                        await _repository.FailedToSendMessages(failedMessages, _state.InstanceId, cancellationToken);
                    }
                }

                messages = await FetchMessages(requestorId, cancellationToken);
            }
        }

        private async Task<IReadOnlyCollection<OutboxMessage>> FetchMessages(Guid requestorId, CancellationToken cancellationToken)
        {
            IReadOnlyList<OutboxMessage> messages = new List<OutboxMessage>();

            if (cancellationToken.IsCancellationRequested) return messages;

            await _repository.BeginTransactionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            try
            {
                await _semaphore.ObtainLock(requestorId, _repository, "MESSAGES", cancellationToken).ConfigureAwait(false);
                messages = await _repository.FetchNextMessages(_outboxTransportOptions.OutboxName, _outboxTransportOptions.PrefetchCount, cancellationToken);

                // Reserve Messages
                if (messages.Any())
                {
                    await _repository.ReserveMessages(messages.Select(x => x.Id), _outboxTransportOptions.OutboxName, _state.InstanceId, cancellationToken).ConfigureAwait(false);
                }
                await _repository.CommitTransactionAsync(false, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                try
                {
                    await _repository.RollbackTransactionAsync(SqlExceptionUtils.IsTransient(e), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ee)
                {
                    // Log failed to rollback and swallow
                    _logger.LogError(ee, "Failed to rollback transaction");
                }
                throw;
            }
            finally
            {
                await _semaphore.ReleaseLock(requestorId, "MESSAGES", cancellationToken).ConfigureAwait(false);
            }

            return messages;
        }

        private async Task<OutboxMessage> SendMessage(OutboxMessage message, CancellationToken cancellationToken)
        {
            try
            {
                var destinationAddress = message.SerializedMessage.Destination;
                var sourceAddress = _bus.Address;

                IPipe<SendContext> sendPipe = CreateMessageContext(sourceAddress, message.SerializedMessage);

                var endpoint = await _bus.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                var outboxed = new Outboxed();

                await endpoint.Send(outboxed, sendPipe, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new OutboxSweeperSendException(message, $"Sweeper failed to send message '{message.Id}' to the transport. See inner exception for more details", e);
            }

            return message;
        }

        IPipe<SendContext> CreateMessageContext(Uri sourceAddress, SerializedMessage message)
        {
            return new SerializedMessageContextAdapter(Pipe.Execute<SendContext>(ctx => ctx.Headers.Set("MT-Outbox-Sweeper", "true")), message, sourceAddress);
        }

        class Outboxed
        {
        }
    }
}
