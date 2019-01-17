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
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Pipeline;
    using Transports;


    /// <summary>
    /// </summary>
    public class EventDataReceiver :
        IEventDataReceiver
    {
        readonly Uri _inputAddress;
        readonly ReceiveEndpointContext _receiveEndpointContext;

        public EventDataReceiver(Uri inputAddress, ReceiveEndpointContext receiveEndpointContext)
        {
            _inputAddress = inputAddress;
            _receiveEndpointContext = receiveEndpointContext;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "brokeredMessage");
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpointContext.ConnectReceiveObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpointContext.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpointContext.ConnectSendObserver(observer);
        }

        async Task IEventDataReceiver.Handle(Microsoft.Azure.EventHubs.EventData message, Action<ReceiveContext> contextCallback)
        {
            var context = new EventDataReceiveContext(_inputAddress, message, _receiveEndpointContext);
            contextCallback?.Invoke(context);

            try
            {
                await _receiveEndpointContext.ReceiveObservers.PreReceive(context).ConfigureAwait(false);
                await _receiveEndpointContext.ReceivePipe.Send(context).ConfigureAwait(false);

                await context.ReceiveCompleted.ConfigureAwait(false);

                await _receiveEndpointContext.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _receiveEndpointContext.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
            }
            finally
            {
                context.Dispose();
            }
        }
        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receiveEndpointContext.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receiveEndpointContext.ReceivePipe.ConnectConsumeObserver(observer);
        }
    }
}
