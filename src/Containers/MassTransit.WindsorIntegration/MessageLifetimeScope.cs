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
namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.MicroKernel.Context;
    using Castle.MicroKernel.Lifestyle.Scoped;


    public class MessageScope :
        IScopeAccessor
    {
        [ThreadStatic]
        static ILifetimeScope _lifetimeScope;

        public void Dispose()
        {
            if (_lifetimeScope != null)
            {
                _lifetimeScope.Dispose();
                _lifetimeScope = null;
            }
        }

        public ILifetimeScope GetScope(CreationContext context)
        {
            if (_lifetimeScope == null)
            {
                throw new InvalidOperationException(
                    "Scope was not available. Did you forget to call MessageScope.BeginScope()?");
            }

            return _lifetimeScope;
        }

        /// <summary>
        /// Called by the WindsorInboundIntercepter to begin the container scope
        /// </summary>
        public static void BeginScope()
        {
            if (_lifetimeScope != null)
                throw new InvalidOperationException("A scope already exists for the current message/thread");

            _lifetimeScope = new DefaultLifetimeScope();
        }

        /// <summary>
        /// Called by the WindsorInboundIntercepter to end the container scope
        /// </summary>
        public static void EndScope()
        {
            if (_lifetimeScope == null)
                throw new InvalidOperationException("A scope was not active for the current message/thread");

            _lifetimeScope.Dispose();
            _lifetimeScope = null;
        }
    }
}