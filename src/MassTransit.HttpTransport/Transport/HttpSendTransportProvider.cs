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
    using Clients;
    using Configuration;
    using Context;
    using MassTransit.Pipeline;
    using Transports;


    public class HttpSendTransportProvider :
        ISendTransportProvider
    {
        readonly IHttpBusConfiguration _busConfiguration;
        readonly ReceiveEndpointContext _receiveEndpointContext;
        readonly IReceivePipe _receivePipe;

        public HttpSendTransportProvider(IHttpBusConfiguration busConfiguration, IReceivePipe receivePipe, ReceiveEndpointContext receiveEndpointContext)
        {
            _busConfiguration = busConfiguration;
            _receivePipe = receivePipe;
            _receiveEndpointContext = receiveEndpointContext;
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            if (!_busConfiguration.Hosts.TryGetHost(address, out var hostConfiguration))
                throw new EndpointNotFoundException($"The host was not found for the specified address: {address}");

            var clientContextSupervisor = new HttpClientContextSupervisor(_receivePipe);

            var sendSettings = address.GetSendSettings();

            return Task.FromResult<ISendTransport>(new HttpSendTransport(clientContextSupervisor, sendSettings, _receiveEndpointContext));
        }
    }
}