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
    using Configuration;
    using Configuration.Configurators;
    using Configuration.Specifications;


    public class RabbitMqSendSettings :
        ExchangeConfigurator,
        SendSettings
    {
        readonly IList<ExchangeBindingPublishTopologySpecification> _exchangeBindings;
        bool _bindToQueue;
        string _queueName;

        public RabbitMqSendSettings(string exchangeName, string exchangeType, bool durable, bool autoDelete)
            : base(exchangeName, exchangeType, durable, autoDelete)
        {
            _exchangeBindings = new List<ExchangeBindingPublishTopologySpecification>();

            QueueArguments = new Dictionary<string, object>();
        }

        bool SendSettings.BindToQueue => _bindToQueue;

        public string QueueName => _queueName;

        public IDictionary<string, object> QueueArguments { get; }

        public IEnumerable<IRabbitMqPublishTopologySpecification> PublishTopologySpecifications => _exchangeBindings;

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

        public void BindToExchange(string exchangeName, Action<IExchangeBindingConfigurator> configure = null)
        {
            ExchangeBindingConfigurator configurator = new ExchangeBindingConfigurator(exchangeName, RabbitMQ.Client.ExchangeType.Fanout, Durable, AutoDelete, "");

            configure?.Invoke(configurator);

            _exchangeBindings.Add(new ExchangeBindingPublishTopologySpecification(configurator));
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

            foreach (var binding in _exchangeBindings)
            {
                yield return $"bindexchange={binding.ExchangeName}";
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