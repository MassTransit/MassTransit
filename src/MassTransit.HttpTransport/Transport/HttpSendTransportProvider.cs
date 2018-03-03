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
    using System.Threading.Tasks;
    using Configuration;
    using Clients;
    using MassTransit.Context;
    using MassTransit.Pipeline;
    using Transports;


    public class HttpSendTransportProvider :
        ISendTransportProvider
    {
        readonly IReceiveObserver _receiveObserver;
        readonly IHttpBusConfiguration _busConfiguration;
        readonly IReceivePipe _receivePipe;
        readonly ReceiveEndpointContext _topology;

        public HttpSendTransportProvider(IHttpBusConfiguration busConfiguration, IReceivePipe receivePipe, IReceiveObserver receiveObserver,
            ReceiveEndpointContext topology)
        {
            _busConfiguration = busConfiguration;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _topology = topology;
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            if (!_busConfiguration.TryGetHost(address, out var hostConfiguration))
                throw new EndpointNotFoundException($"The host was not found for the specified address: {address}");

            var clientCache = new HttpClientCache(_receivePipe);

            var sendSettings = address.GetSendSettings();

            return Task.FromResult<ISendTransport>(new HttpSendTransport(clientCache, sendSettings, _receiveObserver, _topology));
        }
    }
}