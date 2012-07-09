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
namespace MassTransit
{
    using System;
    using System.Transactions;
    using EndpointConfigurators;

    public static class TransactionConfigurationExtensions
    {
        /// <summary>
        /// Sets the default transaction timeout for transactional transports
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static T SetDefaultTransactionTimeout<T>(this T configurator, TimeSpan timeout)
            where T : EndpointFactoryConfigurator
        {
            var builderConfigurator =
                new DelegateEndpointFactoryBuilderConfigurator(x => x.SetDefaultTransactionTimeout(timeout));

            configurator.AddEndpointFactoryConfigurator(builderConfigurator);

            return configurator;
        }

        public static T SetDefaultIsolationLevel<T>(this T configurator, IsolationLevel level)
            where T : EndpointFactoryConfigurator
        {
            var builderConfigurator =
                new DelegateEndpointFactoryBuilderConfigurator(x => x.SetDefaultIsolationLevel(level));

            configurator.AddEndpointFactoryConfigurator(builderConfigurator);

            return configurator;
        }
    }
}