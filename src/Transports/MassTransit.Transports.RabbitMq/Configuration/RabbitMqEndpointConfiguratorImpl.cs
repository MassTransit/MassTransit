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
namespace MassTransit
{
    using Transports.RabbitMq.Configuration;


    public class RabbitMqEndpointConfiguratorImpl :
        RabbitMqEndpointConfigurator
    {
        readonly string _queueName;
        EndpointSettings _settings;

        public RabbitMqEndpointConfiguratorImpl(string queueName)
        {
            _queueName = queueName;
            _settings = new EndpointSettings();
            _settings.QueueName = queueName;
        }

        public RabbitMqEndpointSettings Settings
        {
            get { return _settings; }
        }


        class EndpointSettings :
            RabbitMqEndpointSettings
        {
            public string QueueName { get; set; }
            public bool Durable { get; set; }
            public bool Exclusive { get; set; }
        }


        public void Durable(bool durable = true)
        {
            _settings.Durable = durable;
        }

        public void Exclusive(bool exclusive = true)
        {
            _settings.Exclusive = exclusive;
        }

        public void ConcurrencyLimit(int limit)
        {
            throw new System.NotImplementedException();
        }
    }
}