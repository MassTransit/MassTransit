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
    using System.Collections.Generic;
    using Exceptions;
    using log4net;

    public class CorrelationIdDispatcher<TMessage, TKey> :
        Consumes<TMessage>.Selected,
        Produces<TMessage>
        where TMessage : class, CorrelatedBy<TKey>
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (CorrelationIdDispatcher<TMessage, TKey>));
        private readonly Dictionary<TKey, MessageDispatcher<TMessage>> _dispatchers = new Dictionary<TKey, MessageDispatcher<TMessage>>();

        public bool Active(TKey correlationId)
        {
            MessageDispatcher<TMessage> dispatcher;
            bool found;
            lock (_dispatchers)
                found = _dispatchers.TryGetValue(correlationId, out dispatcher);

            if (found)
                return dispatcher.Active;

            return false;
        }

        public void Consume(TMessage message)
        {
            CorrelatedBy<TKey> correlation = message;

            TKey correlationId = correlation.CorrelationId;

            MessageDispatcher<TMessage> dispatcher;
            bool found;
            lock (_dispatchers)
                found = _dispatchers.TryGetValue(correlationId, out dispatcher);

            if (found)
                dispatcher.Consume(message);
        }

        public bool Accept(TMessage message)
        {
            CorrelatedBy<TKey> correlation = message;

            TKey correlationId = correlation.CorrelationId;

            MessageDispatcher<TMessage> dispatcher;
            bool found;
            lock (_dispatchers)
                found = _dispatchers.TryGetValue(correlationId, out dispatcher);

            if (found)
                return dispatcher.Accept(message);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("No consumers for message type {0} id {1}", typeof (TMessage), correlationId.ToString());

            return false;
        }

        public void Attach(Consumes<TMessage>.All consumer)
        {
            Consumes<TMessage>.For<TKey> correlatedConsumer = GetCorrelatedConsumer(consumer);

            _Attach(correlatedConsumer);
        }

        public void Detach(Consumes<TMessage>.All consumer)
        {
            Consumes<TMessage>.For<TKey> correlatedConsumer = GetCorrelatedConsumer(consumer);

            _Detach(correlatedConsumer);
        }

        private static Consumes<TMessage>.For<TKey> GetCorrelatedConsumer(Consumes<TMessage>.All consumer)
        {
            Consumes<TMessage>.For<TKey> correlatedConsumer = consumer as Consumes<TMessage>.For<TKey>;
            if (correlatedConsumer == null)
                throw new ConventionException(string.Format("The object does not support Consumes<{0}>.For<{1}>", typeof (TMessage), typeof (TKey)));

            return correlatedConsumer;
        }

        public void Consume(object obj)
        {
            Consume((TMessage) obj);
        }

        public void Dispose()
        {
            foreach (MessageDispatcher<TMessage> dispatcher in _dispatchers.Values)
            {
                dispatcher.Dispose();
            }

            _dispatchers.Clear();
        }

        private MessageDispatcher<TMessage> GetDispatcher(TKey correlationId)
        {
            MessageDispatcher<TMessage> dispatcher;
            lock (_dispatchers)
            {
                if (_dispatchers.TryGetValue(correlationId, out dispatcher))
                    return dispatcher;

                dispatcher = new MessageDispatcher<TMessage>();

                _dispatchers.Add(correlationId, dispatcher);

                return dispatcher;
            }
        }

        private void _Attach(Consumes<TMessage>.For<TKey> consumer)
        {
            TKey correlationId = consumer.CorrelationId;

            MessageDispatcher<TMessage> dispatcher = GetDispatcher(correlationId);

            dispatcher.Attach(consumer);
        }

        private void _Detach(Consumes<TMessage>.For<TKey> consumer)
        {
            TKey correlationId = consumer.CorrelationId;

            lock (_dispatchers)
            {
                MessageDispatcher<TMessage> dispatcher = GetDispatcher(correlationId);

                dispatcher.Detach(consumer);

                if (dispatcher.Active == false)
                {
                    _dispatchers.Remove(correlationId);
                    dispatcher.Dispose();
                }
            }
        }
    }
}