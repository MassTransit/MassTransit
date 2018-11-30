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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;
    using Util;


    public class SendEndpointProvider :
        ISendEndpointProvider
    {
        readonly ISendEndpointCache<Uri> _cache;
        readonly SendObservable _observers;
        readonly ISendPipe _sendPipe;
        readonly IReadOnlyDictionary<Type, Uri> _endpointMapping;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;
        readonly ISendTransportProvider _transportProvider;

        public SendEndpointProvider(ISendTransportProvider transportProvider, SendObservable observers, IMessageSerializer serializer, Uri sourceAddress,
            ISendPipe sendPipe, IReadOnlyDictionary<Type, Uri> endpointMapping)
        {
            _transportProvider = transportProvider;
            _serializer = serializer;
            _sourceAddress = sourceAddress;
            _sendPipe = sendPipe;
            _endpointMapping = endpointMapping;

            _cache = new SendEndpointCache<Uri>();
            _observers = observers;
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _cache.GetSendEndpoint(address, CreateSendEndpoint);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Type type)
        {
            if (!_endpointMapping.TryGetValue(type, out var address))
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache.GetShortName(type)} was not found");

            return _cache.GetSendEndpoint(address, CreateSendEndpoint);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        async Task<ISendEndpoint> CreateSendEndpoint(Uri address)
        {
            var sendTransport = await _transportProvider.GetSendTransport(address).ConfigureAwait(false);

            var handle = sendTransport.ConnectSendObserver(_observers);

            return new SendEndpoint(sendTransport, _serializer, address, _sourceAddress, _sendPipe, handle);
        }
    }
}
