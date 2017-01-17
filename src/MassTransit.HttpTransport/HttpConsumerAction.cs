// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.HttpTransport
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Hosting;
    using Internals.Extensions;
    using Logging;
    using Microsoft.Owin;
    using Pipeline;
    using Topology;
    using Transports.Metrics;
    using Util;


    public class HttpConsumerAction :
        HttpConsumerMetrics
    {
        readonly TaskCompletionSource<bool> _deliveryComplete;

        readonly Uri _inputAddress;
        readonly ILog _log = Logger.Get<HttpConsumerAction>();
        readonly ITaskParticipant _participant;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IHttpReceiveEndpointTopology _topology;
        readonly IDeliveryTracker _tracker;
        bool _stopping;

        public HttpConsumerAction(IReceiveObserver receiveObserver, HttpHostSettings settings, IPipe<ReceiveContext> receivePipe, ITaskScope taskSupervisor,
            IHttpReceiveEndpointTopology topology)
        {
            _receiveObserver = receiveObserver;
            _receivePipe = receivePipe;
            _topology = topology;

            _tracker = new DeliveryTracker(OnDeliveryComplete);
            _inputAddress = settings.GetInputAddress();
            _participant = taskSupervisor.CreateParticipant($"{TypeMetadataCache<HttpConsumerAction>.ShortName} - {_inputAddress}", Stop);
            _deliveryComplete = new TaskCompletionSource<bool>();

            _participant.SetReady();
        }

        public long DeliveryCount => _tracker.DeliveryCount;
        public int ConcurrentDeliveryCount => _tracker.MaxConcurrentDeliveryCount;

        public string Route { get; set; }

        public async Task Handle(IOwinContext owinContext, Func<Task> next)
        {
            if (_stopping)
            {
                owinContext.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                owinContext.Response.Write("Stopping");

                await next().ConfigureAwait(false);

                return;
            }

            using (_tracker.BeginDelivery())
            {
                var responseEndpointTopology = _topology.CreateResponseEndpointTopology(owinContext);

                var context = new HttpReceiveContext(owinContext, false, _receiveObserver, responseEndpointTopology);

                try
                {
                    await _receiveObserver.PreReceive(context).ConfigureAwait(false);

                    await _receivePipe.Send(context).ConfigureAwait(false);

                    await context.CompleteTask.ConfigureAwait(false);

                    //TODO: Push into Pipe! -- cant' be on the receive pipe because it doesn't have the content
                    if (!owinContext.Response.ContentLength.HasValue)
                    {
                        owinContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                        owinContext.Response.Write("");
                    }

                    await next().ConfigureAwait(false);

                    await _receiveObserver.PostReceive(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _receiveObserver.ReceiveFault(context, ex).ConfigureAwait(false);

                    //TODO: Push into pipe?
                    owinContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        void OnDeliveryComplete()
        {
            if (_stopping)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("HttpConsumerAction stopped: {0}", _inputAddress);

                _deliveryComplete.TrySetResult(true);
            }
        }

        async Task Stop()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Shutting down consumer: {0}", _inputAddress);

            _stopping = true;

            if (_tracker.ActiveDeliveryCount > 0)
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

            // this has to do something to stop the train, for now, just set complete.

            _participant.SetComplete();

            await _participant.ParticipantCompleted.ConfigureAwait(false);
        }
    }
}