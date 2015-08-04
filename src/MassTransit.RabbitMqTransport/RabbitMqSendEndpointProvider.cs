// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Transports;


    public class RabbitMqSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly Uri _inputAddress;
        readonly SendObservable _sendObservable;
        readonly IMessageSerializer _serializer;
        readonly ISendTransportProvider _transportProvider;

        public RabbitMqSendEndpointProvider(IMessageSerializer serializer, Uri inputAddress, ISendTransportProvider transportProvider)
        {
            _inputAddress = inputAddress;
            _transportProvider = transportProvider;
            _serializer = serializer;
            _sendObservable = new SendObservable();
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            ISendTransport sendTransport = await _transportProvider.GetSendTransport(address);

            sendTransport.ConnectSendObserver(_sendObservable);

            return new SendEndpoint(sendTransport, _serializer, address, _inputAddress);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.ConnectSendObserver(observer);
        }
    }
}