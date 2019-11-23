namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;
    using Util;


    public class RenewLockFilter :
        IFilter<ConsumeContext>
    {
        readonly TimeSpan _delay;

        public RenewLockFilter(TimeSpan delay)
        {
            _delay = delay;
        }

        public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.TryGetPayload(out BrokeredMessageContext brokeredMessageContext);

            var scope = new RenewLockScope(brokeredMessageContext, _delay);
            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                await scope.Complete().ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("renewLock");
        }


        class RenewLockScope
        {
            readonly TaskCompletionSource<bool> _completed;
            readonly BrokeredMessageContext _context;
            readonly TimeSpan _delay;
            readonly CancellationTokenSource _source;

            public RenewLockScope(BrokeredMessageContext context, TimeSpan delay)
            {
                _context = context;
                _delay = delay;
                _source = new CancellationTokenSource();
                _completed = TaskUtil.GetTask<bool>();

                if (context != null)
                {
                    Task.Factory.StartNew(LockRenewal, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
                }
            }

            async Task LockRenewal()
            {
                var delay = _delay;

                if (LockWouldExpireBeforeRenewal())
                {
                    delay = TimeSpan.Zero;
                }

                while (_source.Token.IsCancellationRequested == false)
                {
                    try
                    {
                        await Task.Delay(delay, _source.Token).ConfigureAwait(false);

                        if (_source.Token.IsCancellationRequested)
                            break;

                        await _context.RenewLockAsync().ConfigureAwait(false);

                        LogContext.Debug?.Log("Renewed Lock: {MessageId}", _context.MessageId);

                        delay = _delay;
                    }
                    catch (MessagingCommunicationException exception)
                    {
                        if (exception.IsTransient)
                            delay = TimeSpan.Zero;
                    }
                    catch (MessageLockLostException exception)
                    {
                        _source.Cancel();
                        _completed.TrySetException(exception);

                        LogContext.Warning?.Log(exception, "Lost Message Lock: {MessageId}", _context.MessageId);
                    }
                    catch (SessionLockLostException exception)
                    {
                        _source.Cancel();
                        _completed.TrySetException(exception);

                        LogContext.Warning?.Log(exception, "Lost Message Lock: {MessageId}", _context.MessageId);
                    }
                    catch (MessagingException exception)
                    {
                        if (exception.IsTransient)
                            delay = TimeSpan.Zero;
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (TimeoutException exception)
                    {
                        delay = TimeSpan.Zero;

                        LogContext.Warning?.Log(exception, "Renew Lock Timeout (will retry): {MessageId}", _context.MessageId);
                    }
                    catch (Exception exception)
                    {
                        _source.Cancel();
                        _completed.TrySetException(exception);

                        LogContext.Warning?.Log(exception, "Renew Lock Timeout: {MessageId}", _context.MessageId);
                    }
                }

                _completed.TrySetResult(true);
            }

            bool LockWouldExpireBeforeRenewal()
            {
                return DateTime.UtcNow + _delay >= _context.LockedUntil;
            }

            public Task Complete()
            {
                _source.Cancel();

                return _completed.Task;
            }
        }
    }
}
