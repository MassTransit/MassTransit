// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;
    using Configuration;


    public class RabbitMqSendSettings :
        SendSettings,
        IExchangeConfigurator
    {
        readonly IList<ExchangeBindingSettings> _exchangeBindings;
        bool _bindToQueue;
        IDictionary<string, object> _exchangeArguments;
        IDictionary<string, object> _queueArguments;
        string _queueName;

        public RabbitMqSendSettings(string exchangeName, string exchangeType, bool durable, bool autoDelete)
        {
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            Durable = durable;
            AutoDelete = autoDelete;

            _exchangeBindings = new List<ExchangeBindingSettings>();
        }

        public void SetExchangeArgument(string key, object value)
        {
            if (_exchangeArguments == null)
                _exchangeArguments = new Dictionary<string, object>();

            _exchangeArguments[key] = value;
        }

        public string ExchangeName { get; }

        public bool Durable { get; set; }

        public bool AutoDelete { get; set; }

        public IDictionary<string, object> ExchangeArguments => _exchangeArguments;

        public string ExchangeType { get; set; }

        bool SendSettings.BindToQueue => _bindToQueue;

        public string QueueName => _queueName;

        public IDictionary<string, object> QueueArguments => _queueArguments;

        public IEnumerable<ExchangeBindingSettings> ExchangeBindings => _exchangeBindings;

        public void BindToQueue(string queueName)
        {
            _bindToQueue = true;
            _queueName = queueName;
        }

        public void BindToExchange(string exchangeName)
        {
            var settings = this.GetExchangeBinding(exchangeName);

            _exchangeBindings.Add(settings);
        }

        public void SetQueueArgument(string key, object value)
        {
            if (_queueArguments == null)
                _queueArguments = new Dictionary<string, object>();

            _queueArguments[key] = value;
        }

        IEnumerable<string> GetSettingStrings()
        {
            if (Durable)
                yield return "durable";
            if (AutoDelete)
                yield return "auto-delete";
            if (ExchangeType != RabbitMQ.Client.ExchangeType.Fanout)
                yield return ExchangeType;
            if (_bindToQueue)
                yield return $"bind->{_queueName}";

            if (_exchangeArguments != null)
                foreach (KeyValuePair<string, object> argument in _exchangeArguments)
                {
                    yield return $"e:{argument.Key}={argument.Value}";
                }
            if (_queueArguments != null)
                foreach (KeyValuePair<string, object> argument in _queueArguments)
                {
                    yield return $"q:{argument.Key}={argument.Value}";
                }
        }

        public override string ToString()
        {
            return string.Join(", ", GetSettingStrings());
        }
    }
}