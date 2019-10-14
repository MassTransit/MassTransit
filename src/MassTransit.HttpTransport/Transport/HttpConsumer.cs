// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Microsoft.AspNetCore.Http;
    using Pipeline;
    using Transports.Metrics;


    public sealed class HttpConsumer :
        Agent,
        HttpConsumerMetrics
    {
        readonly TaskCompletionSource<bool> _deliveryComplete;

        readonly HttpReceiveEndpointContext _context;
        readonly IDeliveryTracker _tracker;

        public HttpConsumer(HttpReceiveEndpointContext context)
        {
            _context = context;

            _tracker = new DeliveryTracker(OnDeliveryComplete);
            _deliveryComplete = new TaskCompletionSource<bool>();

            SetReady();
        }

        public long DeliveryCount => _tracker.DeliveryCount;
        public int ConcurrentDeliveryCount => _tracker.MaxConcurrentDeliveryCount;

        public string Route { get; set; }

        public async Task Handle(HttpContext httpContext, Func<Task> next)
        {
            if (IsStopping)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await httpContext.Response.WriteAsync("Stopping").ConfigureAwait(false);

                await next().ConfigureAwait(false);

                return;
            }

            using (_tracker.BeginDelivery())
            {
                var responseEndpointContext = _context.CreateResponseEndpointContext(httpContext);

                var context = new HttpReceiveContext(httpContext, false, responseEndpointContext);

                try
                {
                    await _context.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                    await _context.ReceivePipe.Send(context).ConfigureAwait(false);

                    await context.ReceiveCompleted.ConfigureAwait(false);

                    //TODO: Push into Pipe! -- can't be on the receive pipe because it doesn't have the content
                    if (!httpContext.Response.ContentLength.HasValue)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;
                    }

                    await _context.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);

                    //TODO: ensure the Fault is written to the response pipe
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        void OnDeliveryComplete()
        {
            if (IsStopping)
            {
                _deliveryComplete.TrySetResult(true);
            }
        }

        protected override async Task StopAgent(StopContext context)
        {
            LogContext.Debug?.Log("Stopping Consumer: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
        }

        async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            if (_tracker.ActiveDeliveryCount > 0)
            {
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for consumer cancellation: {InputAddress}", _context.InputAddress);
                }
            }
        }
    }
}
