// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using System.Collections.Generic;
    using System.Net;


    public class RabbitMqSendSettings :
        RabbitMqEntitySettings,
        SendSettings,
        IExchangeConfigurator
    {
        readonly IList<ExchangeBindingSettings> _exchangeBindings;
        bool _bindToQueue;
        string _queueName;

        public RabbitMqSendSettings(string exchangeName, string exchangeType, bool durable, bool autoDelete)
        {
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            Durable = durable;
            AutoDelete = autoDelete;

            _exchangeBindings = new List<ExchangeBindingSettings>();

            QueueArguments = new Dictionary<string, object>();
        }

        bool SendSettings.BindToQueue => _bindToQueue;

        public string QueueName => _queueName;

        public IDictionary<string, object> QueueArguments { get; }

        public IEnumerable<ExchangeBindingSettings> ExchangeBindings => _exchangeBindings;

        public Uri GetSendAddress(Uri hostAddress)
        {
            var builder = new UriBuilder(hostAddress);

            builder.Path = builder.Path == "/"
                ? $"/{ExchangeName}"
                : $"/{string.Join("/", builder.Path.Trim('/'), ExchangeName)}";

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }

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
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
            {
                QueueArguments.Remove(key);
            }
            else
                QueueArguments[key] = value;
        }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            foreach (var option in base.GetQueryStringOptions())
            {
                yield return option;
            }

            if (_bindToQueue)
                yield return "bind=true";
            if (!string.IsNullOrWhiteSpace(QueueName))
                yield return "queue=" + WebUtility.UrlEncode(QueueName);
            if (ExchangeArguments != null && ExchangeArguments.ContainsKey("x-delayed-type"))
                yield return "delayedType=" + ExchangeArguments["x-delayed-type"];

            foreach (var binding in ExchangeBindings)
            {
                yield return $"bindexchange={binding.Exchange.ExchangeName}";
            }
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

            if (ExchangeArguments != null)
                foreach (KeyValuePair<string, object> argument in ExchangeArguments)
                {
                    yield return $"e:{argument.Key}={argument.Value}";
                }
            if (QueueArguments != null)
                foreach (KeyValuePair<string, object> argument in QueueArguments)
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