#nullable enable
namespace MassTransit.JobService;

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Messages;


public class JobProgressBuffer
{
    readonly Channel<ProgressUpdate> _channel;
    readonly INotifyJobContext _notifyJobContext;
    readonly ProgressBufferSettings _settings;

    readonly Task _updateTask;
    long _latestSequenceNumber;

    public JobProgressBuffer(INotifyJobContext notifyJobContext, ProgressBufferSettings? settings = null)
    {
        _notifyJobContext = notifyJobContext;
        _settings = settings ?? new ProgressBufferSettings();

        var channelOptions = new BoundedChannelOptions(_settings.UpdateLimit)
        {
            AllowSynchronousContinuations = false,
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };

        _channel = Channel.CreateBounded<ProgressUpdate>(channelOptions);
        _updateTask = Task.Run(WaitForUpdate);
    }

    public Task Flush()
    {
        _channel.Writer.TryComplete();

        return _updateTask;
    }

    public async Task Update(ProgressUpdate progress, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(progress, cancellationToken).ConfigureAwait(false);
    }

    async Task WaitForUpdate()
    {
        try
        {
            while (await _channel.Reader.WaitToReadAsync().ConfigureAwait(false))
                await ReadUpdate().ConfigureAwait(false);
        }
        catch (ChannelClosedException)
        {
        }
        catch (Exception exception)
        {
            LogContext.Error?.Log(exception, "WaitForUpdate Faulted");
        }
    }

    async Task ReadUpdate()
    {
        var updateToken = new CancellationTokenSource(_settings.TimeLimit);

        try
        {
            ProgressUpdate? latestUpdate = null;
            try
            {
                var updateId = 0;

                while (updateId < _settings.UpdateLimit)
                {
                    if (_channel.Reader.TryRead(out var update))
                    {
                        latestUpdate = update;
                        updateId++;
                    }
                    else if (await _channel.Reader.WaitToReadAsync(updateToken.Token).ConfigureAwait(false) == false)
                        break;
                }
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == updateToken.Token && latestUpdate != null)
            {
            }

            if (latestUpdate.HasValue)
            {
                try
                {
                    await _notifyJobContext.NotifyJobProgress(new SetJobProgressCommand
                    {
                        JobId = latestUpdate.Value.JobId,
                        AttemptId = latestUpdate.Value.AttemptId,
                        SequenceNumber = ++_latestSequenceNumber,
                        Value = latestUpdate.Value.Value,
                        Limit = latestUpdate.Value.Limit
                    }).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    LogContext.Error?.Log(exception, "Unable to update job progress: {JobId} {AttemptId} {SequenceNumber} {Value} {Limit}",
                        latestUpdate?.JobId, latestUpdate?.AttemptId, _latestSequenceNumber, latestUpdate?.Value, latestUpdate?.Limit);
                }
            }
        }
        catch (OperationCanceledException exception) when (exception.CancellationToken == updateToken.Token)
        {
            LogContext.Debug?.Log("operation canceled exception");
        }
        catch (Exception exception)
        {
            LogContext.Error?.Log(exception, "ReadUpdate faulted");
        }
        finally
        {
            updateToken.Dispose();
        }
    }


    public readonly struct ProgressUpdate
    {
        public readonly Guid JobId;
        public readonly Guid AttemptId;
        public readonly long Value;
        public readonly long? Limit;

        public ProgressUpdate(Guid jobId, Guid attemptId, long value, long? limit)
        {
            JobId = jobId;
            AttemptId = attemptId;
            Value = value;
            Limit = limit;
        }
    }
}
