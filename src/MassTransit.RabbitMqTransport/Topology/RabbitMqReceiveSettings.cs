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


    public class RabbitMqReceiveSettings :
        RabbitMqEntitySettings,
        ReceiveSettings,
        IQueueConfigurator,
        IExchangeBindingConfigurator,
        IBindExchangeConfigurator
    {
        public RabbitMqReceiveSettings()
        {
            QueueArguments = new Dictionary<string, object>();

            PrefetchCount = (ushort)Math.Min(Environment.ProcessorCount * 2, 16);

            Exclusive = false;
            RoutingKey = "";

            QueueArguments = new Dictionary<string, object>();
            BindingArguments = new Dictionary<string, object>();
        }

        public RabbitMqReceiveSettings(ReceiveSettings settings)
            : base(settings)
        {
            QueueName = settings.QueueName;
            ExchangeName = settings.ExchangeName;
            PrefetchCount = settings.PrefetchCount;
            Durable = settings.Durable;
            Exclusive = settings.Exclusive;
            AutoDelete = settings.AutoDelete;
            PurgeOnStartup = settings.PurgeOnStartup;
            ExchangeType = settings.ExchangeType;
            QueueArguments = new Dictionary<string, object>(settings.QueueArguments);
            RoutingKey = settings.RoutingKey;
            BindingArguments = new Dictionary<string, object>(settings.BindingArguments);
        }

        public void SetBindingArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                BindingArguments.Remove(key);
            else
                BindingArguments[key] = value;
        }

        string IExchangeConfigurator.ExchangeType
        {
            set { ExchangeType = value; }
        }

        public void SetQueueArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                QueueArguments.Remove(key);
            else
                QueueArguments[key] = value;
        }

        public bool Lazy
        {
            set { SetQueueArgument("x-queue-mode", value ? "lazy" : "default"); }
        }

        public void EnablePriority(byte maxPriority)
        {
            QueueArguments["x-max-priority"] = (int)maxPriority;
        }

        public string RoutingKey { get; set; }

        public string QueueName { get; set; }
        public bool Exclusive { get; set; }
        public ushort PrefetchCount { get; set; }
        public IDictionary<string, object> QueueArguments { get; }
        public IDictionary<string, object> BindingArguments { get; }
        public bool PurgeOnStartup { get; set; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            var builder = new UriBuilder(hostAddress);

            builder.Path = builder.Path == "/"
                ? $"/{QueueName}"
                : $"/{string.Join("/", builder.Path.Trim('/'), QueueName)}";

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            foreach (var option in base.GetQueryStringOptions())
            {
                yield return option;
            }

            if (Exclusive)
                yield return "exclusive=true";
        }
    }
}