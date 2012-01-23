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
namespace MassTransit.SubscriptionConfigurators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Configurators;
	using Magnum.Extensions;
	using SubscriptionBuilders;
	using SubscriptionConnectors;

	public class ConsumerSubscriptionConfiguratorImpl<TConsumer> :
		SubscriptionConfiguratorImpl<ConsumerSubscriptionConfigurator<TConsumer>>,
		ConsumerSubscriptionConfigurator<TConsumer>,
		SubscriptionBuilderConfigurator
		where TConsumer : class
	{
		readonly IConsumerFactory<TConsumer> _consumerFactory;

		public ConsumerSubscriptionConfiguratorImpl(IConsumerFactory<TConsumer> consumerFactory)
		{
			_consumerFactory = consumerFactory;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_consumerFactory == null)
				yield return this.Failure("The consumer factory cannot be null.");


			var warningForMessages = ConsumptionReflector<TConsumer>
				.AllDistinctMessages()
				.Where(x => !(HasDefaultProtectedCtor(typeof (TConsumer)) || HasSinglePublicCtor(typeof (TConsumer))))
				.Select(x => ("You should have a protected default c'tor or a public parameterless" +
				             " c'tor for '{0}', or otherwise you have to ensure that you have" +
				             " no initialization logic in your constructors that NEEDS to run. " +
				             "This is because for other messages, MassTransit will initialize the" +
				             " objects without calling their constructors.").FormatWith(x.MessageType))
				.Select(message => new ValidationResultImpl(ValidationResultDisposition.Warning, 
															"CTorWarning", message))
				;

			foreach (var message in warningForMessages)
				yield return  message;
		}

		private static bool HasDefaultProtectedCtor(Type type)
		{
			return type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Any(c => c.GetParameters().Count() == 0);
		}

		private static bool HasSinglePublicCtor(Type type)
		{
			return
				type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).All(c => c.GetParameters().Count() == 0)
					&& type.GetConstructors().Count() == 1;
		}

		public SubscriptionBuilder Configure()
		{
			return new ConsumerSubscriptionBuilder<TConsumer>(_consumerFactory, ReferenceFactory);
		}
	}
}