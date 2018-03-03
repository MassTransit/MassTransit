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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Topology;
    using Microsoft.ServiceBus.Messaging;
    using Transports;
    using Util;


    /// <summary>
    /// Receives <see cref="EventData"/> from Event Hub
    /// </summary>
    public class EventDataReceiver :
        IEventDataReceiver
    {
        readonly Uri _inputAddress;
        readonly ReceiveObservable _receiveObservers;
        readonly IReceivePipe _receivePipe;
        readonly ReceiveEndpointContext _receiveTopology;

        public EventDataReceiver(Uri inputAddress, IReceivePipe receivePipe, ILog log, ReceiveEndpointContext receiveTopology)
        {
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _receiveTopology = receiveTopology;
            _receiveObservers = new ReceiveObservable();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "brokeredMessage");
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveTopology.PublishEndpointProvider.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            var sendHandle = _receiveTopology.ConnectSendObserver(observer);
            var publishHandle = _receiveTopology.ConnectSendObserver(observer);

            return new MultipleConnectHandle(sendHandle, publishHandle);
        }

        async Task IEventDataReceiver.Handle(EventData message, Action<ReceiveContext> contextCallback)
        {
            var context = new EventDataReceiveContext(_inputAddress, message, _receiveObservers, _receiveTopology);
            contextCallback?.Invoke(context);

            try
            {
                await _receiveObservers.PreReceive(context).ConfigureAwait(false);
                await _receivePipe.Send(context).ConfigureAwait(false);

                await context.CompleteTask.ConfigureAwait(false);

                await _receiveObservers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _receiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
            }
            finally
            {
                context.Dispose();
            }
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receivePipe.ConnectConsumeObserver(observer);
        }
    }
}