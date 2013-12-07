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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using RabbitMQ.Client;


    public class RabbitMqPublisher :
        ConnectionBinding<RabbitMqConnection>
    {
        readonly IRabbitMqEndpointAddress _address;
        readonly object _bindings = new object();
        readonly HashSet<ExchangeBinding> _exchangeBindings;
        readonly IDictionary<string,bool> _exchanges;
        readonly object _lock = new object();
        IModel _channel;

        public RabbitMqPublisher(IRabbitMqEndpointAddress address)
        {
            _address = address;
            _exchangeBindings = new HashSet<ExchangeBinding>();
            _exchanges = new Dictionary<string, bool>();
        }

        public void Bind(RabbitMqConnection connection)
        {
            lock (_lock)
            {
                _channel = connection.Connection.CreateModel();

                RebindExchanges(_channel);
            }
        }

        public void Unbind(RabbitMqConnection connection)
        {
            lock (_lock)
            {
                _channel.Cleanup(200, "Publisher Unbind");
                _channel = null;
            }
        }

        public void ExchangeDeclare(string name, bool temporary)
        {
            lock (_bindings)
            {
                if (_exchanges.ContainsKey(name))
                    return;

                _exchanges[name] = temporary;
            }

            lock (_lock)
                if (_channel != null)
                    DeclareExchange(name, temporary);
        }

        public void ExchangeBind(string destination, string source, bool destinationTemporary = false, bool sourceTemporary = false)
        {
            var binding = new ExchangeBinding(destination, source);

            lock (_bindings)
                if (!_exchangeBindings.Add(binding))
                    return;

            lock(_lock)
            {
                if (_channel != null)
                {
                    ExchangeDeclare(destination, destinationTemporary);
                    ExchangeDeclare(source, sourceTemporary);

                    _channel.ExchangeBind(destination, source, "");
                }
                    
            }
        }

        public void ExchangeUnbind(string destination, string source)
        {
            var binding = new ExchangeBinding(destination, source);

            lock (_bindings)
                _exchangeBindings.Remove(binding);

            try
            {
                lock (_lock)
                {
                    if (_channel != null)
                        _channel.ExchangeUnbind(destination, source, "");
                }
            }
            catch
            {
            }
        }

        void DeclareExchange(string name, bool temporary)
        {
            if (string.Compare(name, _address.Name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                _channel.ExchangeDeclare(_address.Name, ExchangeType.Fanout, _address.Durable,
                    _address.AutoDelete, null);
            }
            else
                _channel.ExchangeDeclare(name, ExchangeType.Fanout, !temporary, temporary, null);
        }

        void RebindExchanges(IModel channel)
        {
            lock (_bindings)
            {
                var exchanges = new Dictionary<string,bool>(_exchanges);
                
                foreach (var binding in _exchangeBindings)
                {
                    if (!exchanges.ContainsKey(binding.Source))
                        exchanges.Add(binding.Source, binding.SourceTemporary);

                    if (!exchanges.ContainsKey(binding.Destination))
                        exchanges.Add(binding.Destination, binding.DestinationTemporary);
                }

                foreach (var exchange in exchanges)
                {
                    DeclareExchange(exchange.Key, exchange.Value);
                }

                foreach (ExchangeBinding exchange in _exchangeBindings)
                    channel.ExchangeBind(exchange.Destination, exchange.Source, "");
            }
        }
    }
}