// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing
{
    using System;
    using System.Threading.Tasks;
    using MessageObservers;


    public class HandlerTestHarness<TMessage>
        where TMessage : class
    {
        readonly ReceivedMessageList<TMessage> _consumed;
        readonly MessageHandler<TMessage> _handler;

        public HandlerTestHarness(BusTestHarness testHarness, MessageHandler<TMessage> handler)
        {
            _handler = handler;

            _consumed = new ReceivedMessageList<TMessage>(testHarness.TestTimeout);

            testHarness.OnConfigureReceiveEndpoint += ConfigureReceiveEndpoint;
        }

        public IReceivedMessageList<TMessage> Consumed => _consumed;

        void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<TMessage>(HandleMessage);
        }

        async Task HandleMessage(ConsumeContext<TMessage> context)
        {
            try
            {
                await _handler(context).ConfigureAwait(false);

                _consumed.Add(context);
            }
            catch (Exception ex)
            {
                _consumed.Add(context, ex);
            }
        }
    }
}