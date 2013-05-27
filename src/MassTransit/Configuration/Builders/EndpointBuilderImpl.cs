// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Builders
{
    using System;
    using Exceptions;
    using Transports;
    using Util;


    public class EndpointBuilderImpl :
        EndpointBuilder
    {
        readonly IEndpointAddress _address;
        readonly ITransportSettings _errorSettings;
        readonly OutboundTransportFactory _errorTransportFactory;
        readonly Func<IInboundMessageTracker> _messageTrackerFactory;
        readonly IEndpointSettings _settings;
        readonly DuplexTransportFactory _transportFactory;

        public EndpointBuilderImpl([NotNull] IEndpointAddress address, [NotNull] IEndpointSettings settings,
            [NotNull] ITransportSettings errorSettings, [NotNull] DuplexTransportFactory transportFactory,
            [NotNull] OutboundTransportFactory errorTransportFactory,
            [NotNull] Func<IInboundMessageTracker> messageTrackerFactory)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _address = address;
            _settings = settings;
            _errorSettings = errorSettings;
            _transportFactory = transportFactory;
            _errorTransportFactory = errorTransportFactory;
            _messageTrackerFactory = messageTrackerFactory;
        }

        public IEndpoint CreateEndpoint(ITransportFactory transportFactory)
        {
            try
            {
                IDuplexTransport transport = _transportFactory(transportFactory, _settings);
                IOutboundTransport errorTransport = _errorTransportFactory(transportFactory, _errorSettings);
                IInboundMessageTracker tracker = _messageTrackerFactory();

                var endpoint = new Endpoint(transport.Address, _settings.Serializer, transport, errorTransport, tracker,
                    _settings.SupportedSerializers);

                return endpoint;
            }
            catch (Exception ex)
            {
                throw new EndpointException(_address.Uri, "Failed to create endpoint", ex);
            }
        }
    }
}