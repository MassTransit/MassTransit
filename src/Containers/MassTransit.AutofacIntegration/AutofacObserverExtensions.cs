// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Autofac;
    using AutofacIntegration;
    using GreenPipes;
    using Pipeline;


    public static class AutofacObserverExtensions
    {
        /// <summary>
        /// Registers an <see cref="IConsumeObserver"/> which resolves the actual observer from the container lifetime scope
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectAutofacConsumeObserver(this IConsumeObserverConnector connector)
        {
            var observer = new AutofacConsumeObserver();

            return connector.ConnectConsumeObserver(observer);
        }

        /// <summary>
        /// Registers an <see cref="IConsumeObserver"/> which resolves the actual observer from the container lifetime scope
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="lifetimeScope">The default lifetime scope</param>
        /// <returns></returns>
        public static ConnectHandle ConnectAutofacConsumeObserver(this IConsumeObserverConnector connector, ILifetimeScope lifetimeScope)
        {
            var observer = new AutofacConsumeObserver(lifetimeScope);

            return connector.ConnectConsumeObserver(observer);
        }

        /// <summary>
        /// Registers an <see cref="IConsumeMessageObserver{T}"/> which resolves the actual observer from the container lifetime scope
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="connector"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectAutofacConsumeMessageObserver<T>(this IConsumeMessageObserverConnector connector)
            where T : class
        {
            var observer = new AutofacConsumeMessageObserver<T>();

            return connector.ConnectConsumeMessageObserver(observer);
        }

        /// <summary>
        /// Registers an <see cref="IConsumeMessageObserver{T}"/> which resolves the actual observer from the container lifetime scope
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="connector"></param>
        /// <param name="lifetimeScope">The default lifetime scope</param>
        /// <returns></returns>
        public static ConnectHandle ConnectAutofacConsumeMessageObserver<T>(this IConsumeMessageObserverConnector connector, ILifetimeScope lifetimeScope)
            where T : class
        {
            var observer = new AutofacConsumeMessageObserver<T>(lifetimeScope);

            return connector.ConnectConsumeMessageObserver(observer);
        }
    }
}