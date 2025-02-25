#nullable enable
namespace MassTransit.SqlTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;
    using Util;


    public class SqlReceiveLockContext :
        MessageRedeliveryContext,
        ReceiveLockContext
    {
        readonly CancellationTokenSource _activeTokenSource;
        readonly ClientContext _clientContext;
        readonly Uri _inputAddress;
        readonly SqlTransportMessage _message;
        readonly Task? _renewLockTask;
        readonly ReceiveSettings _settings;
        readonly DateTime _startedAt;
        bool _locked;

        public SqlReceiveLockContext(Uri inputAddress, SqlTransportMessage message, ReceiveSettings settings, ClientContext clientContext)
        {
            _startedAt = DateTime.UtcNow;
            _inputAddress = inputAddress;
            _message = message;
            _settings = settings;
            _clientContext = clientContext;
            _activeTokenSource = new CancellationTokenSource();
            _locked = true;

            if (_message.LockId.HasValue)
                _renewLockTask = Task.Run(() => RenewLock());
        }

        public async Task ScheduleRedelivery(TimeSpan delay, Action<ConsumeContext, SendContext>? callback)
        {
            if (_locked == false)
                return;

            _activeTokenSource.Cancel();

            try
            {
                if (_renewLockTask != null)
                    await _renewLockTask.ConfigureAwait(false);

                if (!_clientContext.CancellationToken.IsCancellationRequested && _message.LockId != null)
                {
                    var transportHeaders = _message.GetTransportHeaders();

                    var redeliveryCount = transportHeaders.Get(MessageHeaders.RedeliveryCount, default(int?)) ?? 0;

                    transportHeaders.Set(MessageHeaders.RedeliveryCount, redeliveryCount + 1);

                    var unlocked = await _clientContext.Unlock(_message.LockId.Value, _message.MessageDeliveryId, delay, transportHeaders)
                        .ConfigureAwait(false);

                    _locked = false;

                    if (unlocked)
                        LogContext.Debug?.Log("RESEND {DestinationAddress} {MessageId} (delay: {Delay})", _inputAddress, _message.MessageId, delay);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Schedule Redelivery faulted: {DestinationAddress} {MessageId} (lockId: {LockId})", _inputAddress, _message.MessageId,
                    _message.LockId);
            }
            finally
            {
                _activeTokenSource.Dispose();
            }
        }

        public async Task Complete()
        {
            if (_locked == false)
                return;

            _activeTokenSource.Cancel();

            try
            {
                if (_message.LockId.HasValue)
                {
                    await _clientContext.DeleteMessage(_message.LockId.Value, _message.MessageDeliveryId).ConfigureAwait(false);

                    _locked = false;

                    if (_renewLockTask != null)
                        await _renewLockTask.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                LogContext.Warning?.Log(ex, "DeleteMessage failed: {QueueName} {MessageDeliveryId} {LockId}", _settings.EntityName, _message.MessageDeliveryId,
                    _message.LockId);
            }
            finally
            {
                _activeTokenSource.Dispose();
            }
        }

        public async Task Expired()
        {
            if (_locked == false)
                return;

            _activeTokenSource.Cancel();

            try
            {
                if (_message.LockId.HasValue)
                {
                    var transportHeaders = SqlTransportMessage.DeserializeHeaders(_message.TransportHeaders);

                    transportHeaders.Set(MessageHeaders.Reason, "expired");

                    await _clientContext.MoveMessage(_message.LockId.Value, _message.MessageDeliveryId, _settings.QueueName, SqlQueueType.DeadLetterQueue,
                        transportHeaders);

                    _locked = false;

                    if (_renewLockTask != null)
                        await _renewLockTask.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                LogContext.Warning?.Log(ex, "MoveMessage failed: {QueueName} {MessageDeliveryId} {LockId}", _settings.EntityName, _message.MessageDeliveryId,
                    _message.LockId);
            }
            finally
            {
                _activeTokenSource.Dispose();
            }
        }

        public async Task Faulted(Exception exception)
        {
            if (_locked == false)
                return;

            _activeTokenSource.Cancel();

            try
            {
                if (_renewLockTask != null)
                    await _renewLockTask.ConfigureAwait(false);

                if (!_clientContext.CancellationToken.IsCancellationRequested && _message.LockId != null)
                {
                    var headers = _message.GetTransportHeaders();

                    exception = exception.GetBaseException();

                    var exceptionMessage = ExceptionUtil.GetMessage(exception);

                    headers.Set(MessageHeaders.Reason, "fault");
                    headers.Set(MessageHeaders.FaultExceptionType, TypeCache.GetShortName(exception.GetType()));
                    headers.Set(MessageHeaders.FaultMessage, exceptionMessage);
                    headers.Set(MessageHeaders.FaultStackTrace, ExceptionUtil.GetStackTrace(exception));

                    await _clientContext.Unlock(_message.LockId.Value, _message.MessageDeliveryId, _settings.UnlockDelay ?? TimeSpan.Zero, headers)
                        .ConfigureAwait(false);
                }

                _locked = false;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "RenewMessageLock failed: {LockId}, Original Exception: {Exception}", _message.LockId, exception);
            }
            finally
            {
                _activeTokenSource.Dispose();
            }
        }

        public Task ValidateLockStatus()
        {
            if (_locked)
                return Task.CompletedTask;

            throw new TransportException(_inputAddress, $"Message Lock Lost: {_message.LockId}");
        }

        async Task RenewLock()
        {
            TimeSpan CalculateDelay(TimeSpan timeout)
            {
                return TimeSpan.FromSeconds(timeout.TotalSeconds * 0.7);
            }

            var duration = _settings.LockDuration;

            var delay = CalculateDelay(duration);

            duration = TimeSpan.FromSeconds(Math.Min(60, duration.TotalSeconds));

            while (!_activeTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, _activeTokenSource.Token)
                            .ContinueWith(t => t, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
                            .ConfigureAwait(false);
                    }

                    if (_activeTokenSource.IsCancellationRequested)
                        break;

                    if (_message.LockId.HasValue)
                    {
                        if (!await _clientContext.RenewLock(_message.LockId.Value, _message.MessageDeliveryId, duration).ConfigureAwait(false))
                        {
                            LogContext.Warning?.Log("Message Lock Lost: {InputAddress} - {MessageDeliveryId} ({LockId})", _inputAddress,
                                _message.MessageDeliveryId, _message.LockId);

                            _locked = false;

                            break;
                        }
                    }

                    if (DateTime.UtcNow - _startedAt + duration >= _settings.MaxLockDuration)
                        break;

                    delay = CalculateDelay(duration);
                }
                catch (TimeoutException)
                {
                    delay = TimeSpan.Zero;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
    }
}
