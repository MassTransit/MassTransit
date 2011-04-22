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
namespace MassTransit.Internal
{
	using System;
	using Pipeline;

	public class NullServiceBus :
		IServiceBus
	{
		public void Dispose()
		{
		}

		public IEndpoint Endpoint
		{
			get { return new NullEndpoint(); }
		}

		public IEndpoint PoisonEndpoint
		{
            get { return new NullEndpoint(); }
		}

		public UnsubscribeAction Subscribe<T>(Action<T> callback) where T : class
		{
			return () => true;
		}

		public UnsubscribeAction Subscribe<T>(Action<T> callback, Predicate<T> condition) where T : class
		{
			return () => true;
		}

		public UnsubscribeAction Subscribe<T>(T consumer) where T : class
		{
			return () => true;
		}

		public UnsubscribeAction Subscribe<TComponent>() where TComponent : class
		{
			return () => true;
		}

		public UnsubscribeAction Subscribe(Type consumerType)
		{
			return () => true;
		}

		public UnsubscribeAction SubscribeConsumer<T>(Func<T, Action<T>> getConsumerAction) where T : class
		{
			return () => true;
		}

		public void Publish<T>(T message) where T : class
		{
		}

		public TService GetService<TService>()
		{
			throw new System.NotImplementedException();
		}

		public IMessagePipeline OutboundPipeline
		{
			get { throw new System.NotImplementedException(); }
		}

		public IMessagePipeline InboundPipeline
		{
			get { throw new System.NotImplementedException(); }
		}

		public IServiceBus ControlBus
		{
			get { return this; }
		}

	    public IEndpoint GetEndpoint(Uri address)
	    {
	        return new NullEndpoint();
	    }
	}
}