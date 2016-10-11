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
namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Clients;
    using EndpointConfigurators;
    using GreenPipes;
    using Hosting;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Transports;


    public class HttpReceiveEndpointConfigurator :
        ReceiveEndpointConfigurator,
        IHttpReceiveEndpointConfigurator,
        IBusFactorySpecification
    {
        readonly IHttpHost _host;
        readonly HttpReceiveSettings _settings;
        
        public HttpReceiveEndpointConfigurator(IHttpHost host, HttpReceiveSettings settings, IConsumePipe consumePipe = null)
            :
                base(consumePipe)
        {
            _host = host;
            _settings = settings;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result.WithParentKey($"{_host.Settings.ToDebugString()}");
        }

        public void Apply(IBusBuilder builder)
        {
            HttpReceiveEndpointBuilder endpointBuilder = null;
            var receivePipe = CreateReceivePipe(builder, consumePipe =>
            {
                endpointBuilder = new HttpReceiveEndpointBuilder(consumePipe);

                return endpointBuilder;
            });

            if (endpointBuilder == null)
                throw new InvalidOperationException("The endpoint builder was not initialized");

            var transport = new HttpReceiveTransport(_host);

            builder.AddReceiveEndpoint(NewId.Next().ToString(), new ReceiveEndpoint(transport, receivePipe));
        }

        protected override Uri GetInputAddress()
        {
            return _host.Settings.GetInputAddress();
        }

        protected override Uri GetErrorAddress()
        {
            var errorQueueName = "bus_error";
            var sendSettings = new HttpSendSettingsImpl(HttpMethod.Get, errorQueueName);

            //sendSettings.BindToQueue(errorQueueName);

            return _host.Settings.GetSendAddress(sendSettings);
        }

        protected override Uri GetDeadLetterAddress()
        {
            var deadLetterQueueName = "bus_skipped";
            var sendSettings = new HttpSendSettingsImpl(HttpMethod.Delete, deadLetterQueueName);

            //sendSettings.BindToQueue(errorQueueName);

            return _host.Settings.GetSendAddress(sendSettings);
        }
    }
}