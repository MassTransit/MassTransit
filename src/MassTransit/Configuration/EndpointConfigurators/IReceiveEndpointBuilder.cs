// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.EndpointConfigurators
{
    using Pipeline;


    public interface IReceiveEndpointBuilder :
        IConsumeFilterConnector,
        IConsumeObserverConnector

    {
        IInboundPipe InboundPipe { get; }
    }


    public class ReceiveEndpointBuilder :
        IReceiveEndpointBuilder
    {
        readonly IInboundPipe _inboundPipe;

        public ReceiveEndpointBuilder(IInboundPipe inboundPipe)
        {
            _inboundPipe = inboundPipe;
        }

        public ConnectHandle Connect<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _inboundPipe.Connect(pipe);
        }

        public ConnectHandle Connect<TMessage>(IConsumeObserver<TMessage> observer)
            where TMessage : class
        {
            return _inboundPipe.Connect(observer);
        }

        public IInboundPipe InboundPipe
        {
            get { return _inboundPipe; }
        }
    }
}