namespace MassTransit.AmazonSqsTransport;

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
    readonly CancellationTokenSource _activeTokenSource;
    readonly ClientContext _clientContext;
    readonly CancellationToken _cancellationToken;
    readonly Uri _inputAddress;
    readonly Message _message;

    readonly ReceiveSettings _settings;
    readonly DateTime _startedAt;
    readonly Task _visibilityTask;
    bool _locked;

    public AmazonSqsReceiveLockContext(Uri inputAddress, Message message, ReceiveSettings settings, ClientContext clientContext,
        CancellationToken cancellationToken)
    {
        _startedAt = DateTime.UtcNow;
        _inputAddress = inputAddress;
        _message = message;
        _settings = settings;
        _clientContext = clientContext;
        _cancellationToken = cancellationToken;
        _activeTokenSource = new CancellationTokenSource();
        _locked = true;

        _visibilityTask = Task.Run(() => RenewMessageVisibility());
    }

    public async Task Complete()
    {
        _activeTokenSource.Cancel();

        try
        {
            await _clientContext.DeleteMessage(_settings.EntityName, _message.ReceiptHandle, _cancellationToken).ConfigureAwait(false);

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
        if (_activeTokenSource?.IsCancellationRequested is false)
            _activeTokenSource.Cancel();

        try
        {
            await _visibilityTask.ConfigureAwait(false);

            if (!_clientContext.CancellationToken.IsCancellationRequested && _settings.QueueUrl != null)
            {
                await _clientContext.ChangeMessageVisibility(_settings.QueueUrl, _message.ReceiptHandle, _settings.RedeliverVisibilityTimeout,
                    _cancellationToken).ConfigureAwait(false);
            }

            _locked = false;
        }
        catch (OperationCanceledException)
        {
        }
        catch (MessageNotInflightException)
        {
            _locked = false;
        }
        catch (ReceiptHandleIsInvalidException)
        {
            _locked = false;
        }
        catch (Exception ex)
        {
            LogContext.Error?.Log(ex, "ChangeMessageVisibility failed: {ReceiptHandle}, Original Exception: {Exception}", _message.ReceiptHandle, exception);
        }
        finally
        {
            _activeTokenSource?.Dispose();
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

                if (_settings.QueueUrl != null)
                    await _clientContext
                        .ChangeMessageVisibility(_settings.QueueUrl, _message.ReceiptHandle, visibilityTimeout, _cancellationToken)
                        .ConfigureAwait(false);

                if (DateTime.UtcNow - _startedAt.AddSeconds(visibilityTimeout) >= _settings.MaxVisibilityTimeout)
                {
                    LogContext.Warning?.Log("Maximum visibility timeout {MaxVisibilityTimeout} for message {ReceiptHandle} exceeded.",
                        _settings.MaxVisibilityTimeout, _message.ReceiptHandle);
                    break;
                }

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
