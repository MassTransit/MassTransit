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
namespace MassTransit.Registration
{
    /// <summary>
    /// Supports retrieval of services used by the ConfigurationRegistry 
    /// </summary>
    public interface IConfigurationServiceProvider
    {
        /// <summary>
        /// Get the requested service or throw an exception that it wasn't available
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetRequiredService<T>()
            where T : class;

        /// <summary>
        /// Returns the service, if available, otherwise returns null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>()
            where T : class;
    }
}
