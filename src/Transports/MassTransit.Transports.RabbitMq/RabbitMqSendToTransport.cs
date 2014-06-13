// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Globalization;
    using System.Threading.Tasks;
    using Magnum;


    public interface RabbitMqSendSettings
    {
    }


    public class RabbitMqSentContext<T> :
        SentContext<T>
        where T : class
    {
        
    }

    public class RabbitMqSendToTransport :
        ISendToTransport
    {
        readonly string _exchange;
        readonly IHaModel _model;

        public RabbitMqSendToTransport(IHaModel model, string exchange)
        {
            _model = model;
            _exchange = exchange;
        }

        public async Task<SentContext<T>> Send<T>(T message, Func<SendContext<T>, Task<SendContext<T>>> callback)
            where T : class
        {
            var properties = _model.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>();

            var context = new RabbitMqSendContextImpl<T>(properties, message, _exchange);

            await callback(context);

            properties.SetPersistent(context.Durable);
    
            if(context.MessageId.HasValue)
                properties.MessageId = context.MessageId.ToString();

            if (context.CorrelationId.HasValue)
                properties.CorrelationId = context.CorrelationId.ToString();

            if (context.TimeToLive.HasValue)
                properties.Expiration = context.TimeToLive.Value.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);

            properties.Headers["Content-Type"] = context.ContentType;

//                            properties.Headers = context.Headers.ToDictionary(entry => entry.Key, entry => (object)entry.Value);

            await _model.BasicPublishAsync(context.Exchange, context.RoutingKey, context.Mandatory, context.Immediate,
                context.BasicProperties,
                context.Body);

            return new RabbitMqSentContext<T>();
        }
    }
}