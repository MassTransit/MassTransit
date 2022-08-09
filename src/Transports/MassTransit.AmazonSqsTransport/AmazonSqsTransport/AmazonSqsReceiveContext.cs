namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Internals;
    using Transports;


    public sealed class AmazonSqsReceiveContext :
        BaseReceiveContext,
        AmazonSqsMessageContext,
        ReceiveLockContext
    {
        static readonly TimeSpan MaxVisibilityTimeout = TimeSpan.FromHours(12);

        readonly CancellationTokenSource _activeTokenSource;
        readonly ClientContext _clientContext;
        readonly SqsReceiveEndpointContext _context;
        readonly Message _message;
        readonly DateTime _receiveTime;
        readonly ReceiveSettings _settings;
        bool _locked;
        bool _initialVisibilityTimeoutChecked;


        public AmazonSqsReceiveContext(Message message, bool redelivered, DateTime receiveTime,
            SqsReceiveEndpointContext context, ClientContext clientContext,
            ReceiveSettings settings, ConnectionContext connectionContext)
            : base(redelivered, context, settings, clientContext, connectionContext)
        {
            _context = context;
            _clientContext = clientContext;
            _message = message;
            _receiveTime = receiveTime;
            _settings = settings;

            Body = new StringMessageBody(message?.Body);

            _activeTokenSource = new CancellationTokenSource();
            _locked = true;
        }

        protected override IHeaderProvider HeaderProvider => new AmazonSqsHeaderProvider(TransportMessage);

        public override MessageBody Body { get; }

        public Message TransportMessage => _message;

        public Dictionary<string, MessageAttributeValue> Attributes => _message.MessageAttributes;

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
                await _clientContext.ChangeMessageVisibility(_settings.QueueUrl, _message.ReceiptHandle, _settings.RedeliverVisibilityTimeout)
                    .ConfigureAwait(false);

                _locked = false;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "ChangeMessageVisibility failed: {ReceiptHandle}, Original Exception: {Exception}", TransportMessage.ReceiptHandle,
                    exception);
            }
        }

        public Task ValidateLockStatus()
        {
            if (_locked)
            {
                if (!_initialVisibilityTimeoutChecked)
                {
                    _initialVisibilityTimeoutChecked = true;

                    if (_receiveTime + TimeSpan.FromSeconds(_settings.VisibilityTimeout) <= DateTime.UtcNow)
                    {
                        _locked = false;

                        throw new AmazonSqsMessageVisibilityTimeoutExpiredException(_context.InputAddress,
                            $"The message visibility timeout expired: {_message.MessageId}");
                    }

                    Task.Factory.StartNew(RenewMessageVisibility, _activeTokenSource.Token, TaskCreationOptions.None, TaskScheduler.Default);
                }

                return Task.CompletedTask;
            }

            throw new TransportException(_context.InputAddress, $"Message Lock Lost: {TransportMessage.ReceiptHandle}");
        }

        public override void Dispose()
        {
            _activeTokenSource.Dispose();

            base.Dispose();
        }

        async Task RenewMessageVisibility()
        {
            TimeSpan CalculateDelay(int timeout)
            {
                return TimeSpan.FromSeconds(timeout * 0.7);
            }

            var visibilityTimeout = _settings.VisibilityTimeout;

            var delay = CalculateDelay(visibilityTimeout);

            var elapsedTimeSinceReceive = DateTime.UtcNow - _receiveTime;
            if (elapsedTimeSinceReceive >= delay)
            {
                delay = TimeSpan.Zero;
            }
            else
            {
                delay = CalculateDelay((int)delay.TotalSeconds);
            }

            visibilityTimeout = Math.Min(60, visibilityTimeout);

            while (_activeTokenSource.Token.IsCancellationRequested == false)
            {
                try
                {
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, _activeTokenSource.Token).ConfigureAwait(false);

                    if (_activeTokenSource.Token.IsCancellationRequested)
                        break;

                    await _clientContext.ChangeMessageVisibility(_settings.QueueUrl, TransportMessage.ReceiptHandle, visibilityTimeout)
                        .ConfigureAwait(false);

                    // Max 12 hours, https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/sqs-visibility-timeout.html
                    if ((DateTime.UtcNow - _receiveTime) + TimeSpan.FromSeconds(visibilityTimeout) >= MaxVisibilityTimeout)
                        break;

                    delay = CalculateDelay(visibilityTimeout);
                }
                catch (MessageNotInflightException exception)
                {
                    LogContext.Warning?.Log(exception, "Message no longer in flight: {ReceiptHandle}", TransportMessage.ReceiptHandle);

                    _locked = false;

                    Cancel();
                    break;
                }
                catch (ReceiptHandleIsInvalidException exception)
                {
                    LogContext.Warning?.Log(exception, "Message receipt handle is invalid: {ReceiptHandle}", TransportMessage.ReceiptHandle);

                    _locked = false;

                    Cancel();
                    break;
                }
                catch (AmazonSQSException exception)
                {
                    LogContext.Error?.Log(exception, "Failed to extend message {ReceiptHandle} visibility to {VisibilityTimeout} ({ElapsedTime})",
                        TransportMessage.ReceiptHandle, TimeSpan.FromSeconds(visibilityTimeout).ToFriendlyString(), ElapsedTime);

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
