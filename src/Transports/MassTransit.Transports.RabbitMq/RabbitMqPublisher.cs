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
    using System.Collections.Generic;
    using System.Linq;
    using RabbitMQ.Client;

    public class RabbitMqPublisher :
        ConnectionBinding<RabbitMqConnection>
    {
        readonly HashSet<ExchangeBinding> _exchangeBindings;
        readonly HashSet<string> _exchanges;
        IModel _channel;

        public RabbitMqPublisher()
        {
            _exchangeBindings = new HashSet<ExchangeBinding>();
            _exchanges = new HashSet<string>();
        }

        public void Bind(RabbitMqConnection connection)
        {
            _channel = connection.Connection.CreateModel();

            RebindExchanges(_channel);
        }

        public void Unbind(RabbitMqConnection connection)
        {
            if (_channel != null)
            {
                if (_channel.IsOpen)
                    _channel.Close(200, "publisher unbind");
                _channel.Dispose();
                _channel = null;
            }
        }

        public void ExchangeDeclare(string name)
        {
            lock (_exchangeBindings)
                _exchanges.Add(name);

            try
            {
                if (_channel != null)
                    _channel.ExchangeDeclare(name, ExchangeType.Fanout, true, false, null);
            }
            catch
            {
            }
        }

        public void ExchangeBind(string destination, string source)
        {
            var binding = new ExchangeBinding(destination, source);

            lock (_exchangeBindings)
                _exchangeBindings.Add(binding);

            try
            {
                if (_channel != null)
                {
                    _channel.ExchangeDeclare(source, ExchangeType.Fanout, true, false, null);
                    _channel.ExchangeDeclare(destination, ExchangeType.Fanout, true, false, null);
                    _channel.ExchangeBind(destination, source, "");
                }
            }
            catch
            {
            }
        }

        public void ExchangeUnbind(string destination, string source)
        {
            var binding = new ExchangeBinding(destination, source);

            lock (_exchangeBindings)
                _exchangeBindings.Remove(binding);

            try
            {
                if (_channel != null)
                {
                    _channel.ExchangeUnbind(destination, source, "");
                }
            }
            catch
            {
            }
        }

        void RebindExchanges(IModel channel)
        {
            lock (_exchangeBindings)
            {
                IEnumerable<string> exchanges = _exchangeBindings.Select(x => x.Destination)
                                                                 .Concat(_exchangeBindings.Select(x => x.Source))
                                                                 .Concat(_exchanges)
                                                                 .Distinct();

                foreach (string exchange in exchanges)
                {
                    channel.ExchangeDeclare(exchange, ExchangeType.Fanout, true, false, null);
                }

                foreach (ExchangeBinding exchange in _exchangeBindings)
                {
                    channel.ExchangeBind(exchange.Destination, exchange.Source, "");
                }
            }
        }
    }
}