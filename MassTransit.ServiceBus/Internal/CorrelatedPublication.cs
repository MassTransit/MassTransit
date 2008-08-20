// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.Internal
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using log4net;
    using Subscriptions;

    public class CorrelatedPublication<TMessage, TKey> :
        IPublicationTypeInfo
        where TMessage : class, CorrelatedBy<TKey>
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageTypePublication<TMessage>));
        private readonly Type _keyType;
        private readonly Type _messageType;

        public CorrelatedPublication()
        {
            _keyType = typeof (TKey);
            _messageType = typeof (TMessage);
        }

        public IList<Subscription> GetConsumers<T>(IDispatcherContext context, T message) where T : class
        {
            CorrelatedBy<TKey> key = message as CorrelatedBy<TKey>;
            if (key == null)
                throw new ConventionException(string.Format("Object of type {0} is not correlated by type {1}", typeof (T), _keyType));

            return GetConsumers(context.SubscriptionCache, key);
        }

        public void PublishFault<T>(IServiceBus bus, Exception ex, T message) where T : class
        {
            TMessage msg = message as TMessage;
            if (msg == null)
                throw new ConventionException(string.Format("Object of type {0} is not of type {1}", typeof (T), _messageType));

            Fault<TMessage, TKey> fault = new Fault<TMessage, TKey>(ex, msg);

            try
            {
                bus.Publish(fault);
            }
            catch (Exception fex)
            {
                _log.Error("Failed to publish Fault<" + typeof (TMessage).Name + "> message for exception", fex);
            }
        }

        public IList<Subscription> GetConsumers(ISubscriptionCache cache, CorrelatedBy<TKey> key)
        {
            if (cache == null)
                return new List<Subscription>();

            return cache.List(_messageType.FullName, key.CorrelationId.ToString());
        }
    }
}