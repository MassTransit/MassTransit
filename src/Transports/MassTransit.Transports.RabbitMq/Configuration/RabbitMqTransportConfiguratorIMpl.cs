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
namespace MassTransit.Transports.RabbitMq.Configuration
{
    using System;
    using System.Collections.Generic;
    using EndpointConfigurators;
    using MassTransit.Configurators;


    public class RabbitMqTransportConfigurator :
        IRabbitMqTransportConfigurator,
        ITransportBuilder
    {
        readonly IList<ReceiveSettings> _receiveSettings; 
        readonly IList<RabbitMqHostSettings> _hosts;
        readonly IRabbitMqReceiveEndpointConfigurator _defaultEndpointConfigurator;
        readonly RabbitMqPublishSettings _publishSettings;

        public RabbitMqTransportConfigurator(ITransportSelector selector)
        {
            _hosts = new List<RabbitMqHostSettings>();
            _defaultEndpointConfigurator = new RabbitMqReceiveEndpointConfigurator();
            _publishSettings = new RabbitMqPublishSettings();
            _receiveSettings = new List<ReceiveSettings>();

            selector.SelectTransport(this);
        }

        public void Host(RabbitMqHostSettings settings)
        {
            _hosts.Add(settings);
        }

        public void Endpoint(RabbitMqEndpointSettings settings)
        {
            throw new NotImplementedException();
        }

        public void ReceiveEndpoint(ReceiveSettings receiveSettings)
        {
            if (receiveSettings == null)
                throw new ArgumentNullException("receiveSettings");

            _receiveSettings.Add(receiveSettings);
        }

        public void Mandatory(bool mandatory = true)
        {
            _publishSettings.Mandatory = mandatory;

        }

        public void OnPublish<T>(Action<RabbitMqPublishContext<T>> callback) 
            where T : class
        {
            throw new NotImplementedException();
        }

        public void OnPublish(Action<RabbitMqPublishContext> callback)
        {
            throw new NotImplementedException();
        }

        public IServiceBus Build()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;

        }
    }
}