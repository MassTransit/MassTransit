// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Configuration
{
    using System;

    public class CrazyAdvancedShit :
        AdvancedConfiguration
    {
        readonly EndpointResolverConfigurator _epc;
        readonly ServiceBusConfigurator _sbc;

        public CrazyAdvancedShit(EndpointResolverConfigurator epc, ServiceBusConfigurator sbc)
        {
            _epc = epc;
            _sbc = sbc;
        }

        public void ConfigureEndpoint(string uriString, Action<IEndpointConfigurator> action)
        {
            ConfigureEndpoint(new Uri(uriString), action);
        }

        public void ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> action)
        {
            _epc.ConfigureEndpoint(uri, action);
        }

        public void SetConcurrentConsumerLimit(int concurrentConsumerLimit)
        {
            _sbc.SetConcurrentConsumerLimit(concurrentConsumerLimit);
        }

        public void SetConcurrentReceiverLimit(int concurrentReceiverLimit)
        {
            _sbc.SetConcurrentReceiverLimit(concurrentReceiverLimit);
        }
    }
}