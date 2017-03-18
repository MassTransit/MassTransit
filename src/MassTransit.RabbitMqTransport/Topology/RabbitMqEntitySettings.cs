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
    using Configurators;


    public static class RabbitMqEntityExtensions
    {
        public static IEnumerable<string> GetQueryStringOptions(this ExchangeConfigurator exchange)
        {
            if (!exchange.Durable)
                yield return "durable=false";
            if (exchange.AutoDelete)
                yield return "autodelete=true";
            if (exchange.ExchangeType != RabbitMQ.Client.ExchangeType.Fanout)
                yield return "type=" + exchange.ExchangeType;
        }

        public static IEnumerable<string> GetQueryStringOptions(this QueueConfigurator exchange)
        {
            if (!exchange.Durable)
                yield return "durable=false";
            if (exchange.AutoDelete)
                yield return "autodelete=true";
            if (exchange.ExchangeType != RabbitMQ.Client.ExchangeType.Fanout)
                yield return "type=" + exchange.ExchangeType;
        }
    }
}