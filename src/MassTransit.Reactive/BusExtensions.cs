// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;


    public static class BusExtensions
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
        public static IObserver<T> AsObserver<T>(this IBus bus)
            where T : class
        {
            return Observer.Create<T>(value => bus.Publish(value));
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
        public static IObserver<T> AsObserver<T>(this IBus bus, Action<PublishContext<T>> contextCallback)
            where T : class
        {
            return Observer.Create<T>(value => bus.Publish(value, Pipe.Execute(contextCallback)));
        }

        public static IObservable<T> AsObservable<T>(this IBus bus)
            where T : class
        {
            return Observable.Create<T>(observer => new ObserverConnection<T>(bus, observer));
        }

        public static IObservable<ConsumeContext<T>> AsObservableContext<T>(this IBus bus)
            where T : class
        {
            return Observable.Create<ConsumeContext<T>>(observer => new ObserverConnection<T>(bus, observer));
        }
    }
}