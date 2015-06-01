// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class RabbitMqSendSettings :
        SendSettings
    {
        readonly bool _autoDelete;
        readonly bool _durable;
        readonly string _exchangeName;
        readonly string _exchangeType;
        bool _bindToQueue;
        IDictionary<string, object> _exchangeArguments;
        IDictionary<string, object> _queueArguments;
        string _queueName;

        public RabbitMqSendSettings(string exchangeName, string exchangeType, bool durable, bool autoDelete)
        {
            _exchangeName = exchangeName;
            _exchangeType = exchangeType;
            _durable = durable;
            _autoDelete = autoDelete;
        }

        public string ExchangeName
        {
            get { return _exchangeName; }
        }

        public bool Durable
        {
            get { return _durable; }
        }

        public bool AutoDelete
        {
            get { return _autoDelete; }
        }

        public IDictionary<string, object> ExchangeArguments
        {
            get { return _exchangeArguments; }
        }

        public string ExchangeType
        {
            get { return _exchangeType; }
        }

        bool SendSettings.BindToQueue
        {
            get { return _bindToQueue; }
        }

        public string QueueName
        {
            get { return _queueName; }
        }

        public IDictionary<string, object> QueueArguments
        {
            get { return _queueArguments; }
        }

        public void BindToQueue(string queueName)
        {
            _bindToQueue = true;
            _queueName = queueName;
        }

        public void SetQueueArgument(string key, object value)
        {
            if (_queueArguments == null)
                _queueArguments = new Dictionary<string, object>();

            _queueArguments[key] = value;
        }

        public void SetExchangeArgument(string key, object value)
        {
            if (_exchangeArguments == null)
                _exchangeArguments = new Dictionary<string, object>();

            _exchangeArguments[key] = value;
        }
    }
}