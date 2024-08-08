namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Internals;
    using Transports;


    public class AmazonSqsReceiveLockContext :
        ReceiveLockContext
    {
        static readonly TimeSpan MaxVisibilityTimeout = TimeSpan.FromHours(12);
        readonly CancellationTokenSource _activeTokenSource;
        readonly ClientContext _clientContext;
        readonly Uri _inputAddress;
        readonly Message _message;

        readonly ReceiveSettings _settings;
        readonly DateTime _startedAt;
        readonly Task _visibilityTask;
        bool _locked;

        public AmazonSqsReceiveLockContext(Uri inputAddress, Message message, ReceiveSettings settings, ClientContext clientContext)
        {
            _startedAt = DateTime.UtcNow;
            _inputAddress = inputAddress;
            _message = message;
            _settings = settings;
            _clientContext = clientContext;
            _activeTokenSource = new CancellationTokenSource();
            _locked = true;

            _visibilityTask = Task.Run(() => RenewMessageVisibility());
        }

        public async Task Complete()
        {
            _activeTokenSource.Cancel();

            try
            {
                await _clientContext.DeleteMessage(_settings.EntityName, _message.ReceiptHandle).ConfigureAwait(false);

                await _visibilityTask.ConfigureAwait(false);
            }
            catch (MessageNotInflightException)
            {
                _locked = false;
                throw;
            }
            catch (ReceiptHandleIsInvalidException)
            {
                _locked = false;
                throw;
            }
            finally
            {
                _activeTokenSource.Dispose();
            }
        }

        public async Task Faulted(Exception exception)
        {
            _activeTokenSource.Cancel();

            try
            {
                await _visibilityTask.ConfigureAwait(false);

                if (!_clientContext.CancellationToken.IsCancellationRequested)
                {
                    await _clientContext.ChangeMessageVisibility(_settings.QueueUrl, _message.ReceiptHandle, _settings.RedeliverVisibilityTimeout)
                        .ConfigureAwait(false);
                }

                _locked = false;
            }
            catch (OperationCanceledException)
            {
            }
            catch (MessageNotInflightException)
            {
                _locked = false;
                throw;
            }
            catch (ReceiptHandleIsInvalidException)
            {
                _locked = false;
                throw;
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "ChangeMessageVisibility failed: {ReceiptHandle}, Original Exception: {Exception}",
                    _message.ReceiptHandle, exception);
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

            throw new TransportException(_inputAddress, $"Message Lock Lost: {_message.ReceiptHandle}");
        }

        async Task RenewMessageVisibility()
        {
            TimeSpan CalculateDelay(int timeout)
            {
                return TimeSpan.FromSeconds(timeout * 0.7);
            }

            var visibilityTimeout = _settings.VisibilityTimeout;

            var delay = CalculateDelay(visibilityTimeout);

            visibilityTimeout = Math.Min(60, visibilityTimeout);

            while (_locked && !_activeTokenSource.IsCancellationRequested)
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

                    await _clientContext.ChangeMessageVisibility(_settings.QueueUrl, _message.ReceiptHandle, visibilityTimeout).ConfigureAwait(false);

                    // Max 12 hours, https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/sqs-visibility-timeout.html
                    if (DateTime.UtcNow - _startedAt.AddSeconds(visibilityTimeout) >= MaxVisibilityTimeout)
                        break;

                    delay = CalculateDelay(visibilityTimeout);
                }
                catch (MessageNotInflightException exception)
                {
                    LogContext.Warning?.Log(exception, "Message no longer in flight: {ReceiptHandle}", _message.ReceiptHandle);

                    _locked = false;

                    break;
                }
                catch (ReceiptHandleIsInvalidException exception)
                {
                    LogContext.Warning?.Log(exception, "Message receipt handle is invalid: {ReceiptHandle}", _message.ReceiptHandle);

                    _locked = false;

                    break;
                }
                catch (AmazonSQSException exception)
                {
                    LogContext.Error?.Log(exception, "Failed to extend message {ReceiptHandle} visibility to {VisibilityTimeout} ({ElapsedTime})",
                        _message.ReceiptHandle, TimeSpan.FromSeconds(visibilityTimeout).ToFriendlyString(), DateTime.UtcNow - _startedAt);

                    break;
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
