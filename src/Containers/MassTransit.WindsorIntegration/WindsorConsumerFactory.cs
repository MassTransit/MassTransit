// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.WindsorIntegration
{
	using System;
	using System.Collections.Generic;
	using Castle.MicroKernel.Lifestyle;
	using Castle.Windsor;
	using Exceptions;
	using Pipeline;

	public class WindsorConsumerFactory<T> :
		IConsumerFactory<T>
		where T : class
	{
		readonly IWindsorContainer _container;

		public WindsorConsumerFactory(IWindsorContainer container)
		{
			_container = container;
		}

		public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(
			IConsumeContext<TMessage> context, InstanceHandlerSelector<T, TMessage> selector)
			where TMessage : class
		{
		    using (_container.BeginScope())
		    {
		        var consumer = _container.Resolve<T>();
		        if (consumer == null)
		            throw new ConfigurationException(string.Format("Unable to resolve type '{0}' from container: ", typeof(T)));

		        try
		        {
		            foreach (var handler in selector(consumer, context))
		            {
		                yield return handler;
		            }
		        }
		        finally
		        {
		            _container.Release(consumer);
		        }
		    }
		}
	}
}