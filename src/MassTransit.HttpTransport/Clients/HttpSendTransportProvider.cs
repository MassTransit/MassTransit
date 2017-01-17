// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HttpTransport.Clients
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Topology;
    using Transport;
    using Transports;
    using Transports.InMemory;
    using Transports.InMemory.Topology;


    public class HttpSendTransportProvider :
        ISendTransportProvider
    {
        readonly BusHostCollection<HttpHost> _hosts;
        readonly IReceiveObserver _receiveObserver;
        readonly IReceivePipe _receivePipe;
        readonly IReceiveEndpointTopology _topology;

        public HttpSendTransportProvider(BusHostCollection<HttpHost> hosts, IReceivePipe receivePipe, IReceiveObserver receiveObserver,
            IInMemoryEndpointConfiguration configuration, Uri inputAddress, IMessageSerializer serializer, IHttpHost host)
        {
            _hosts = hosts;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _topology = new HttpReceiveEndpointTopology(configuration, inputAddress, serializer, this, host);
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            var sendSettings = address.GetSendSettings();

            var host = _hosts.GetHost(address);

            var clientCache = new HttpClientCache(_hosts[0].Supervisor, _receivePipe);

            return Task.FromResult<ISendTransport>(new HttpSendTransport(clientCache, sendSettings, _receiveObserver, _topology));
        }
    }
}