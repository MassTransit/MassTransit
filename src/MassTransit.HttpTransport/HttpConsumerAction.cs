namespace MassTransit.HttpTransport
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Hosting;
    using Internals.Extensions;
    using Logging;
    using Microsoft.Owin;
    using Pipeline;
    using Util;


    public class HttpConsumerAction : HttpConsumerMetrics
    {
        readonly ILog _log = Logger.Get<HttpConsumerAction>();

        readonly Uri _inputAddress;
        readonly IReceiveObserver _receiveObserver;
        readonly HttpHostSettings _settings;
        readonly IPipe<ReceiveContext> _receivePipe;
        int _currentPendingDeliveryCount;
        int _deliveryCount;
        bool _shuttingDown;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly ITaskParticipant _participant;
        int _maxPendingDeliveryCount;
        readonly ConcurrentDictionary<Guid, HttpReceiveContext> _pending;

        public HttpConsumerAction(IReceiveObserver receiveObserver, HttpHostSettings settings,IPipe<ReceiveContext> receivePipe,
            ITaskScope taskSupervisor)
        {
            _receiveObserver = receiveObserver;
            _settings = settings;
            _receivePipe = receivePipe;

            _pending = new ConcurrentDictionary<Guid, HttpReceiveContext>();
            _inputAddress = settings.GetInputAddress();
            _participant = taskSupervisor.CreateParticipant($"{TypeMetadataCache<HttpConsumerAction>.ShortName} - {_inputAddress}", Stop);
            _deliveryComplete = new TaskCompletionSource<bool>();

            _participant.SetReady();

        }

        public long DeliveryCount { get; }

        public string Route { get; set; }

        public long ConcurrentDeliveryCount { get; set; }

        public string ConsumerTag => Guid.Empty.ToString();

        public async Task Handle(IOwinContext owinContext, Func<Task> next)
        {
            Guid deliveryTag = NewId.NextGuid();

            if (_shuttingDown)
            {
                owinContext.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                owinContext.Response.Write("SHUTTING DOWN");
                await next();
            }

            Interlocked.Increment(ref _deliveryCount);

            int current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            var inputAddress = owinContext.Request.Uri;
            var body = owinContext.Request.Body;
            var headers = new HttpHeaderProvider(owinContext.Request.Headers);

            var context = new HttpReceiveContext(inputAddress, body, headers, false, _receiveObserver);
            
            context.GetOrAddPayload(() => owinContext.Request);
            context.GetOrAddPayload(() => owinContext.Response);

            try
            {
                if (!_pending.TryAdd(deliveryTag, context))
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("Duplicate BasicDeliver: {0}", deliveryTag);
                }

                await _receiveObserver.PreReceive(context).ConfigureAwait(false);

                await _receivePipe.Send(context).ConfigureAwait(false);

                await context.CompleteTask.ConfigureAwait(false);

                //TODO: is this a good ack replacement
                owinContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                owinContext.Response.Write("DELIVERED");
                await next();

                await _receiveObserver.PostReceive(context).ConfigureAwait(false);
                
            }
            catch (Exception ex)
            {
                await _receiveObserver.ReceiveFault(context, ex).ConfigureAwait(false);

                owinContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                owinContext.Response.Write("ERROR");
            }
            finally
            {
                HttpReceiveContext ignored;
                _pending.TryRemove(deliveryTag, out ignored);

                int pendingCount = Interlocked.Decrement(ref _currentPendingDeliveryCount);
                if (pendingCount == 0 && _shuttingDown)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Consumer shutdown completed: {0}", _inputAddress);

                    _deliveryComplete.TrySetResult(true);
                }
            }

            await next();
        }

        async Task Stop()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Shutting down consumer: {0}", _inputAddress);

            _shuttingDown = true;

            if (_currentPendingDeliveryCount > 0)
            {
                try
                {
                    using (var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
                    {
                        await _deliveryComplete.Task.WithCancellation(cancellation.Token).ConfigureAwait(false);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (_log.IsWarnEnabled)
                        _log.WarnFormat("Timeout waiting for consumer to exit: {0}", _inputAddress);
                }
            }

            await _participant.ParticipantCompleted.ConfigureAwait(false);
        }
    }
}