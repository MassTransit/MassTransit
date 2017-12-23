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
namespace MassTransit.AutofacIntegration
{
    using Autofac;


    /// <summary>
    /// A lifetime scope registry contains an indexed set of lifetime scopes that can be used on 
    /// a per-index basis as the root for additional lifetime scopes (per request, etc.)
    /// </summary>
    public interface ILifetimeScopeRegistry<TId> :
        ILifetimeScope
    {
        /// <summary>
        /// Returns the lifetime scope for the specified scopeId
        /// </summary>
        /// <param name="scopeId">The scope identifier</param>
        /// <returns>The lifetime scope</returns>
        ILifetimeScope GetLifetimeScope(TId scopeId);

        /// <summary>
        /// Specify the configuration method for a lifetime scope
        /// </summary>
        /// <param name="scopeId">The switch identifier</param>
        /// <param name="configureCallback">The configuration action for the switch</param>
        void ConfigureLifetimeScope(TId scopeId, LifetimeScopeConfigurator<TId> configureCallback);
    }
}