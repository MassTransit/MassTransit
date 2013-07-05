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
namespace MassTransit.EndpointConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using Builders;
    using Configurators;
    using Magnum.Extensions;
    using Serialization;
    using Transports;
    using Util;


    public class EndpointConfiguratorImpl :
        EndpointConfigurator,
        EndpointFactoryBuilderConfigurator
    {
        readonly Uri _baseUri;
        readonly EndpointSettings _settings;
        IEndpointAddress _errorAddress;
        OutboundTransportFactory _errorTransportFactory;
        DuplexTransportFactory _transportFactory;

        public EndpointConfiguratorImpl([NotNull] IEndpointAddress address,
            [NotNull] IEndpointFactoryDefaultSettings defaultSettings)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (defaultSettings == null)
                throw new ArgumentNullException("defaultSettings");

            _baseUri = new Uri(address.Uri.GetLeftPart(UriPartial.Path));

            _transportFactory = DefaultTransportFactory;
            _errorTransportFactory = DefaultErrorTransportFactory;

            _settings = defaultSettings.CreateEndpointSettings(address);
        }

        public EndpointConfigurator UseSerializer(IMessageSerializer serializer)
        {
            _settings.Serializer = serializer;
            return this;
        }

        public EndpointConfigurator UseSupportedSerializers(ISupportedMessageSerializers serializers)
        {
            _settings.SupportedSerializers = serializers;
            return this;
        }

        public EndpointConfigurator SetErrorAddress(Uri uri)
        {
            _errorAddress = new EndpointAddress(uri);
            return this;
        }

        public EndpointConfigurator PurgeExistingMessages()
        {
            _settings.PurgeExistingMessages = true;
            return this;
        }

        public EndpointConfigurator SetInboundMessageTrackerFactory(MessageTrackerFactory messageTrackerFactory)
        {
            _settings.TrackerFactory = messageTrackerFactory;
            return this;
        }

        public EndpointConfigurator SetTransactionTimeout(TimeSpan timeout)
        {
            _settings.TransactionTimeout = timeout;
            return this;
        }

        public EndpointConfigurator SetIsolationLevel(IsolationLevel isolationLevel)
        {
            _settings.IsolationLevel = isolationLevel;
            return this;
        }

        public EndpointConfigurator CreateTransactional()
        {
            _settings.Transactional = true;
            return this;
        }

        public EndpointConfigurator CreateIfMissing()
        {
            _settings.CreateIfMissing = true;
            return this;
        }

        public EndpointConfigurator SetMessageRetryLimit(int retryLimit)
        {
            _settings.RetryLimit = retryLimit;
            return this;
        }

        public EndpointConfigurator SetTransportFactory(DuplexTransportFactory transportFactory)
        {
            _transportFactory = transportFactory;
            return this;
        }

        public EndpointConfigurator SetErrorTransportFactory(OutboundTransportFactory errorTransportFactory)
        {
            _errorTransportFactory = errorTransportFactory;
            return this;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_errorAddress != null)
            {
                if (string.Compare(_errorAddress.Uri.Scheme, _settings.Address.Uri.Scheme,
                    StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    yield return this.Failure("ErrorAddress", _errorAddress.ToString(),
                        "The error address ('{0}') must use the same scheme as the endpoint address ('{1}')"
                            .FormatWith(_errorAddress.Uri, _settings.Address.Uri.Scheme));
                }
                else
                    yield return this.Success("ErrorAddress", "Using specified error address: " + _errorAddress);
            }

            if (_transportFactory == null)
                yield return this.Failure("TransportFactory", "The transport factory method is null");

            if (_errorTransportFactory == null)
                yield return this.Failure("ErrorTransportFactory", "The error transport factory method is null");
        }

        public EndpointFactoryBuilder Configure(EndpointFactoryBuilder builder)
        {
            EndpointBuilder endpointBuilder = CreateBuilder();

            builder.AddEndpointBuilder(_baseUri, endpointBuilder);

            return builder;
        }

        public EndpointBuilder CreateBuilder()
        {
            ITransportSettings errorSettings = new TransportSettings(_errorAddress ?? _settings.ErrorAddress, _settings);

            var endpointBuilder = new EndpointBuilderImpl(_settings.Address, _settings, errorSettings, _transportFactory,
                _errorTransportFactory, () => _settings.TrackerFactory(_settings.RetryLimit));

            return endpointBuilder;
        }

        static IDuplexTransport DefaultTransportFactory(ITransportFactory transportFactory, ITransportSettings settings)
        {
            return transportFactory.BuildLoopback(settings);
        }

        static IOutboundTransport DefaultErrorTransportFactory(ITransportFactory transportFactory,
            ITransportSettings settings)
        {
            return transportFactory.BuildError(settings);
        }
    }
}