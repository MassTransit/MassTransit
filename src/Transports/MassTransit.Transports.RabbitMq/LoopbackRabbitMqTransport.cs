// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports.RabbitMq
{
    using System;

    public class LoopbackRabbitMqTransport :
        IDuplexTransport
    {
        readonly IInboundTransport _inbound;
        readonly IOutboundTransport _outbound;

        public LoopbackRabbitMqTransport(IEndpointAddress address, IInboundTransport inbound, IOutboundTransport outbound)
        {
            _inbound = inbound;
            _outbound = outbound;
            Address = address;
        }

        public IEndpointAddress Address { get; private set; }

        public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
        {
            _inbound.Receive(callback, timeout);
        }

        public void Send(Action<ISendContext> callback)
        {
            _outbound.Send(callback);
        }

        public IOutboundTransport OutboundTransport
        {
            get { return _outbound; }
        }

        public IInboundTransport InboundTransport
        {
            get { return _inbound; }
        }

        public void Dispose()
        {
            _inbound.Dispose();
            _outbound.Dispose();
        }
    }
}