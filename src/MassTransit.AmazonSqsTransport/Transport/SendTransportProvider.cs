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
namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Configuration.Configuration;
    using Transports;


    public class SendTransportProvider :
        ISendTransportProvider
    {
        readonly IAmazonSqsReceiveEndpointConfiguration _configuration;

        public SendTransportProvider(IAmazonSqsReceiveEndpointConfiguration configuration)
        {
            _configuration = configuration;
        }

        Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            if (!_configuration.BusConfiguration.TryGetHost(address, out var hostConfiguration))
            {
                var hostAddress = _configuration.Host.Address;
                var builder = new UriBuilder(address) {Host = hostAddress.Host, Port = hostAddress.Port};

                if (!_configuration.BusConfiguration.TryGetHost(builder.Uri, out hostConfiguration))
                    throw new EndpointNotFoundException($"The host was not found for the specified address: {address}");
            }

            return hostConfiguration.CreateSendTransport(address);
        }
    }
}