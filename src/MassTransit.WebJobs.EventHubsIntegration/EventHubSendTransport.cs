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
namespace MassTransit.WebJobs.EventHubsIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Microsoft.Azure.EventHubs;
    using Pipeline.Observables;
    using Transports;


    /// <summary>
    /// Send message to an Event Hub
    /// </summary>
    public class EventHubSendTransport :
        Supervisor,
        ISendTransport
    {
        readonly Uri _address;
        readonly SendObservable _observers;

        readonly IPipeContextSource<EventDataSendEndpointContext> _source;

        public EventHubSendTransport(IPipeContextSource<EventDataSendEndpointContext> source, Uri address)
        {
            _source = source;
            _address = address;
            _observers = new SendObservable();
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            IPipe<EventDataSendEndpointContext> clientPipe = Pipe.ExecuteAsync<EventDataSendEndpointContext>(async clientContext =>
            {
                var context = new EventDataSendContext<T>(message, cancellationToken);

                try
                {
                    await pipe.Send(context).ConfigureAwait(false);

                    await _observers.PreSend(context).ConfigureAwait(false);

                    using (var eventData = new EventData(context.Body))
                    {
                        eventData.Properties.SetTextHeaders(context.Headers, (_, text) => text);

                        // if (context.PartitionKey != null)
                        //     eventData.PartitionKey = context.PartitionKey;

                        await clientContext.Send(eventData).ConfigureAwait(false);

                        context.LogSent();

                        await _observers.PostSend(context).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    await _observers.SendFault(context, ex).ConfigureAwait(false);

                    throw;
                }
            });

            return _source.Send(clientPipe, cancellationToken);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        protected override Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping Transport: {Address}", _address);

            return base.StopSupervisor(context);
        }
    }
}
