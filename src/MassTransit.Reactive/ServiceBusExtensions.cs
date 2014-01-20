// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Reactive
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using SubscriptionConfigurators;


    public static class ServiceBusExtensions
    {
        /// <summary>
        ///     <para>
        ///         Gets an observer that publishes messages to all subscribed consumers for
        ///         the message type as specified by the generic parameter.
        ///     </para>
        ///     <para>
        ///         Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        ///     </para>
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="bus">The message bus</param>
        public static IObserver<T> AsObserver<T>(this IServiceBus bus) where T : class
        {
            return bus.AsObserver<T>(
                contextCallback =>
                    {
                    });
        }

        /// <summary>
        ///     <para>
        ///         Gets an observer that publishes messages to all subscribed consumers for
        ///         the message type as specified by the generic parameter. The second parameter
        ///         allows the caller to customize the outgoing publish context and set things
        ///         like headers on the message.
        ///     </para>
        ///     <para>
        ///         Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        ///     </para>
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="bus">The message bus</param>
        /// <param name="contextCallback">
        ///     A callback that gives the caller
        ///     access to the publish context.
        /// </param>
        public static IObserver<T> AsObserver<T>(this IServiceBus bus, Action<IPublishContext<T>> contextCallback) where T : class
        {
            return Observer.Create<T>(
                value => bus.Publish(value, contextCallback));
        }

        public static IObservable<T> AsObservable<T>(this IServiceBus bus) where T : class
        {
            return Observable.Create<T>(
                observer => new ServiceBusSubscription<T>(bus, observer, null));
        }

        public static IObservable<T> AsObservable<T>(this IServiceBus bus, Predicate<T> condition) where T : class
        {
            return Observable.Create<T>(
                observer => new ServiceBusSubscription<T>(bus, observer, condition));
        }

        /// <summary>
        /// Subscribe an observer to a message on the service bus
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="observer">The observer to subscribe</param>
        /// <returns>The subscription configurator</returns>
        public static InstanceSubscriptionConfigurator Observe<T>(this SubscriptionBusServiceConfigurator configurator,
            IObserver<T> observer)
            where T : class
        {
            var consumer = new ObserverInstanceConsumer<T>(observer);

            return configurator.Instance(consumer);
        }
    }
}