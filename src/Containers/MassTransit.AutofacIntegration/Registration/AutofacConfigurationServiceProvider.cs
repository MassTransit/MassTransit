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
namespace MassTransit.AutofacIntegration.Registration
{
    using Autofac;
    using MassTransit.Registration;


    public class AutofacConfigurationServiceProvider :
        IConfigurationServiceProvider
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacConfigurationServiceProvider(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public T GetService<T>()
            where T : class
        {
            return _lifetimeScope.Resolve<T>();
        }
    }
}
