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
namespace MassTransit.SubscriptionConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Distributor;
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using Pipeline;
    using Saga;
    using Util;

    public interface ConsumerConnector
    {
        UnsubscribeAction Connect<TConsumer>(IInboundPipelineConfigurator configurator, IConsumerFactory<TConsumer> consumerFactory) 
            where TConsumer : class;
    }

    public class ConsumerConnector<T> :
        ConsumerConnector
        where T : class
    {
        readonly object[] _args;
    	readonly IEnumerable<ConsumerSubscriptionConnector> _connectors;

    	public ConsumerConnector()
        {
            _args = new object[] {};

            Type[] interfaces = typeof (T).GetInterfaces();

            if (interfaces.Contains(typeof (ISaga)))
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            if (interfaces.Implements(typeof (InitiatedBy<>))
                || interfaces.Implements(typeof (Orchestrates<>))
                || interfaces.Implements(typeof (Observes<,>)))
                throw new ConfigurationException("InitiatedBy, Orchestrates, and Observes can only be used with sagas");

            if (interfaces.Implements(typeof (IDistributor<>))
                || interfaces.Implements(typeof (IWorker<>))
                || interfaces.Implements(typeof (ISagaWorker<>)))
                throw new ConfigurationException("Distributor classes can only be subscribed as instances");

            _connectors = ConsumesSelectedContext()
                .Concat(ConsumesContext())
                .Concat(ConsumesSelected())
                .Concat(ConsumesAll())
                .Distinct((x, y) => x.MessageType == y.MessageType)
                .ToList();
        }

        public IEnumerable<ConsumerSubscriptionConnector> Connectors
        {
            get { return _connectors; }
        }

        public UnsubscribeAction Connect<TConsumer>(IInboundPipelineConfigurator configurator, IConsumerFactory<TConsumer> consumerFactory ) 
            where TConsumer : class
        {
            return _connectors.Select(x => x.Connect(configurator, consumerFactory))
                .Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x());
        }

        IEnumerable<ConsumerSubscriptionConnector> ConsumesContext()
        {
			return ConsumptionReflector<T>.ConsumesContextMessages()
                .Select(x =>
                        FastActivator.Create(typeof (ContextConsumerSubscriptionConnector<,>),
                            new[] {typeof (T), x.MessageType}, _args))
                .Cast<ConsumerSubscriptionConnector>();
        }

        IEnumerable<ConsumerSubscriptionConnector> ConsumesSelectedContext()
        {
			return ConsumptionReflector<T>.ConsumesSelectedContextMessages()
                .Select(x =>
                        FastActivator.Create(typeof (SelectedContextConsumerSubscriptionConnector<,>),
                            new[] {typeof (T), x.MessageType}, _args))
                .Cast<ConsumerSubscriptionConnector>();
        }

        IEnumerable<ConsumerSubscriptionConnector> ConsumesAll()
        {
			return ConsumptionReflector<T>.ConsumesAllMessages()
                .Select(x =>
                        FastActivator.Create(typeof (ConsumerSubscriptionConnector<,>),
                            new[] {typeof (T), x.MessageType}, _args))
                .Cast<ConsumerSubscriptionConnector>();
        }
        IEnumerable<ConsumerSubscriptionConnector> ConsumesSelected()
        {
			return ConsumptionReflector<T>.ConsumesSelectedMessages()
                .Select(x =>
                        FastActivator.Create(typeof (SelectedConsumerSubscriptionConnector<,>),
                            new[] {typeof (T), x.MessageType}, _args))
                .Cast<ConsumerSubscriptionConnector>();
        }
    }

	/// <summary>
	/// Helper class for providing the message reflection for consumers.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal static class ConsumptionReflector<T>
		where T : class
	{
		internal static IEnumerable<ConsumptionPair> AllDistinctMessages()
		{
			return ConsumesSelectedContextMessages()
				.Concat(ConsumesContextMessages())
				.Concat(ConsumesSelectedMessages())
				.Concat(ConsumesAllMessages())
				.Distinct((x, y) => x.MessageType == y.MessageType);
		}

		internal static IEnumerable<ConsumptionPair> ConsumesContextMessages()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.All))
				.Select(x => new ConsumptionPair(x, x.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsGenericType)
				.Where(x => x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>))
				.Select(x => new ConsumptionPair(x.InterfaceType, x.MessageType.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsValueType == false);
		}

		internal static IEnumerable<ConsumptionPair> ConsumesSelectedContextMessages()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.Selected))
				.Select(x => new ConsumptionPair(x, x.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsGenericType)
				.Where(x => x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>))
				.Select(x => new ConsumptionPair(x.InterfaceType, x.MessageType.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsValueType == false);
		}

		internal static IEnumerable<ConsumptionPair> ConsumesAllMessages()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.All))
				.Select(x => new ConsumptionPair(x, x.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x =>
					   !(x.MessageType.IsGenericType &&
						 x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)));
		}

		internal static IEnumerable<ConsumptionPair> ConsumesSelectedMessages()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.Selected))
				.Select(x => new ConsumptionPair(x, x.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x =>
					   !(x.MessageType.IsGenericType &&
						 x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)));
		}
	}

	internal class ConsumptionPair
	{
		public ConsumptionPair(Type interfaceType, Type messageType)
		{
			InterfaceType = interfaceType;
			MessageType = messageType;
		}

		public Type InterfaceType { get; private set; }
		public Type MessageType { get; private set; }
	}
}