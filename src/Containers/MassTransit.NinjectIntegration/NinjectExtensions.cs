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
namespace MassTransit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;
	using Ninject;
	using NinjectIntegration;
	using Saga;
	using Saga.SubscriptionConfigurators;
	using SubscriptionConfigurators;
	using Util;

    public static class NinjectExtensions
	{
		public static void LoadFrom(this SubscriptionBusServiceConfigurator configurator, IKernel kernel)
		{
            var ex = new NotImplementedByNinjectException("Ninject does not support this option. See https://github.com/ninject/ninject/issues/35");
		    
            ex.HelpLink = "https://github.com/ninject/ninject/issues/35";

		    throw ex;
		}

		public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(
			this SubscriptionBusServiceConfigurator configurator, IKernel kernel)
			where TConsumer : class
		{
			var consumerFactory = new NinjectConsumerFactory<TConsumer>(kernel);

			return configurator.Consumer(consumerFactory);
		}

		public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
			this SubscriptionBusServiceConfigurator configurator, IKernel kernel)
			where TSaga : class, ISaga
		{
			return configurator.Saga(kernel.Get<ISagaRepository<TSaga>>());
		}

		private static IList<Type> FindTypes<T>(IKernel kernel, Func<Type, bool> filter)
		{
			return kernel.GetBindings(typeof (T))
				.Select(x => x.Service)
				.Distinct()
				.Where(filter)
				.ToList();
		}
	}

    public class NotImplementedByNinjectException : NotImplementedException
    {
        public NotImplementedByNinjectException()
        {
        }

        public NotImplementedByNinjectException(string message)
            : base(message)
        {
        }

        public NotImplementedByNinjectException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NotImplementedByNinjectException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}