// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionConfigurator :
        IRegistrationConfigurator
    {
        IServiceCollection Collection { get; }

        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IServiceProvider, IBusControl> busFactory);

        /// <summary>
        /// Add a request client, for the request type, which uses the <see cref="ConsumeContext"/> if present, otherwise
        /// uses the <see cref="IBus"/>. The request is published, unless an endpoint convention is specified for the
        /// request type.
        /// </summary>
        /// <param name="timeout">The request timeout</param>
        /// <typeparam name="T">The request message type</typeparam>
        void AddRequestClient<T>(RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Add a request client, for the request type, which uses the <see cref="ConsumeContext"/> if present, otherwise
        /// uses the <see cref="IBus"/>.
        /// </summary>
        /// <param name="destinationAddress">The destination address for the request</param>
        /// <param name="timeout">The request timeout</param>
        /// <typeparam name="T">The request message type</typeparam>
        void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class;
    }
}
