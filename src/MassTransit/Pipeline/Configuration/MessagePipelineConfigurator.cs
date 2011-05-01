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
namespace MassTransit.Pipeline.Configuration
{
    using System;
    using Sinks;
    using Util;

    public class MessagePipelineConfigurator :
        IPipelineConfigurator,
        IDisposable
    {
    	private readonly UnsubscribeAction _emptyToken = () => true;
        private volatile bool _disposed;

        private RegistrationList<ISubscriptionEvent> _subscriptionEventHandlers = new RegistrationList<ISubscriptionEvent>();
        private MessagePipeline _pipeline;
    	private IServiceBus _bus;

    	private MessagePipelineConfigurator(IServiceBus bus)
        {
    		_bus = bus;

            var router = new MessageRouter<object>();

            _pipeline = new MessagePipeline(router, this);
		}

    	public UnregisterAction Register(ISubscriptionEvent subscriptionEventHandler)
        {
            return _subscriptionEventHandlers.Register(subscriptionEventHandler);
        }

    	public IMessagePipeline Pipeline
    	{
    		get { return _pipeline; }
    	}

    	public IServiceBus Bus
    	{
			get { return _bus; }
    	}

    	public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            _pipeline = null;
            _subscriptionEventHandlers = null;

            _disposed = true;
        }

        ~MessagePipelineConfigurator()
        {
            Dispose(false);
        }

    	public static MessagePipeline CreateDefault(IServiceBus bus)
        {
            return new MessagePipelineConfigurator(bus)._pipeline;
        }

        public UnsubscribeAction SubscribedTo<T>() where T : class
        {
            UnsubscribeAction result = () => true;

            _subscriptionEventHandlers.Each(x => { result += x.SubscribedTo<T>(); });

            return result;
        }

        public UnsubscribeAction SubscribedTo<T, K>(K correlationId) where T : class, CorrelatedBy<K>
        {
            UnsubscribeAction result = () => true;

            _subscriptionEventHandlers.Each(x => { result += x.SubscribedTo<T, K>(correlationId); });

            return result;
        }
    }
}