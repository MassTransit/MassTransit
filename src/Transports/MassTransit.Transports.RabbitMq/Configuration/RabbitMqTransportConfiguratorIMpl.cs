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
    using MassTransit.Configurators;


    public class RabbitMqTransportConfiguratorImpl :
        RabbitMqTransportConfigurator,
        ITransportBuilder
    {
        readonly ITransportSelector _selector;
        readonly IList<RabbitMqHostSettings> _hosts;

        public RabbitMqTransportConfiguratorImpl()
        {
            _hosts = new List<RabbitMqHostSettings>();
        }

        public RabbitMqTransportConfiguratorImpl(ITransportSelector selector)
        {
            _selector = selector;
        }

        public void Host(RabbitMqHostSettings settings)
        {
            _hosts.Add(settings);
        }

        public void Endpoint(RabbitMqEndpointSettings settings)
        {
            throw new NotImplementedException();
        }

        public void Mandatory(bool mandatory = true)
        {
            throw new NotImplementedException();
        }

        public void OnPublish<T>(Action<RabbitMqPublishContext<T>> callback) where T : class
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