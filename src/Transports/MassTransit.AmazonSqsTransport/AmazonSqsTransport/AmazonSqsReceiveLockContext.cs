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
        ReceiveLockContext,
        IAsyncDisposable
    {
        static readonly TimeSpan MaxVisibilityTimeout = TimeSpan.FromHours(12);
        readonly CancellationTokenSource _activeTokenSource;
        readonly ClientContext _clientContext;
        readonly Message _message;

        readonly BaseReceiveContext _receiveContext;
        readonly ReceiveSettings _settings;
        readonly Task<Task> _visibilityTask;
        bool _locked;

        public AmazonSqsReceiveLockContext(BaseReceiveContext receiveContext, Message message, ReceiveSettings settings, ClientContext clientContext)
        {
            _receiveContext = receiveContext;
            _message = message;
            _settings = settings;
            _clientContext = clientContext;
            _activeTokenSource = new CancellationTokenSource();
            _locked = true;

            _visibilityTask = Task.Factory.StartNew(RenewMessageVisibility, _activeTokenSource.Token, TaskCreationOptions.None, TaskScheduler.Default);
        }

        public async ValueTask DisposeAsync()
        {
            _activeTokenSource.Cancel();
            await _visibilityTask.ConfigureAwait(false);
            _activeTokenSource.Dispose();
        }

        public Task Complete()
        {
            _activeTokenSource.Cancel();

            return _clientContext.DeleteMessage(_settings.EntityName, _message.ReceiptHandle);
        }

        public async Task Faulted(Exception exception)
        {
            _activeTokenSource.Cancel();

            try
            {
                if (!_clientContext.CancellationToken.IsCancellationRequested)
                {
                    await _clientContext.ChangeMessageVisibility(_settings.QueueUrl, _message.ReceiptHandle,
                        _settings.RedeliverVisibilityTimeout).ConfigureAwait(false);
                }

                _locked = false;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "ChangeMessageVisibility failed: {ReceiptHandle}, Original Exception: {Exception}",
                    _message.ReceiptHandle, exception);
            }
        }

        public Task ValidateLockStatus()
        {
            if (_locked)
                return Task.CompletedTask;

            throw new TransportException(_receiveContext.InputAddress, $"Message Lock Lost: {_message.ReceiptHandle}");
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

            while (_activeTokenSource.Token.IsCancellationRequested == false)
            {
                try
                {
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, _activeTokenSource.Token).ConfigureAwait(false);

                    if (_activeTokenSource.Token.IsCancellationRequested)
                        break;

                    await _clientContext.ChangeMessageVisibility(_settings.QueueUrl, _message.ReceiptHandle, visibilityTimeout)
                        .ConfigureAwait(false);

                    // Max 12 hours, https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/sqs-visibility-timeout.html
                    if (_receiveContext.ElapsedTime + TimeSpan.FromSeconds(visibilityTimeout) >= MaxVisibilityTimeout)
                        break;

                    delay = CalculateDelay(visibilityTimeout);
                }
                catch (MessageNotInflightException exception)
                {
                    LogContext.Warning?.Log(exception, "Message no longer in flight: {ReceiptHandle}", _message.ReceiptHandle);

                    _locked = false;

                    _receiveContext.Cancel();
                    break;
                }
                catch (ReceiptHandleIsInvalidException exception)
                {
                    LogContext.Warning?.Log(exception, "Message receipt handle is invalid: {ReceiptHandle}", _message.ReceiptHandle);

                    _locked = false;

                    _receiveContext.Cancel();
                    break;
                }
                catch (AmazonSQSException exception)
                {
                    LogContext.Error?.Log(exception, "Failed to extend message {ReceiptHandle} visibility to {VisibilityTimeout} ({ElapsedTime})",
                        _message.ReceiptHandle, TimeSpan.FromSeconds(visibilityTimeout).ToFriendlyString(),
                        _receiveContext.ElapsedTime);

                    break;
                }
                catch (TimeoutException)
                {
                    delay = TimeSpan.Zero;
                }
                catch (OperationCanceledException)
                {
                    _activeTokenSource.Cancel();
                }
                catch (Exception)
                {
                    _activeTokenSource.Cancel();
                }
            }
        }
    }
}
