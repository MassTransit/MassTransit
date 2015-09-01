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
namespace RapidTransit
{
    using System;
    using Autofac;
    using Topshelf.Logging;
    using Topshelf.Runtime;


    public abstract class TopshelfServiceBootstrapper<T> :
        IDisposable
        where T : TopshelfServiceBootstrapper<T>
    {
        readonly IContainer _container;
        readonly LogWriter _log = HostLogger.Get<T>();

        protected TopshelfServiceBootstrapper(HostSettings hostSettings)
        {
            _log.InfoFormat("Configuring Service Container");

            var builder = new ContainerBuilder();

            builder.RegisterInstance(hostSettings);

            ConfigureContainer(builder);

            builder.RegisterType<TopshelfHostService>()
                .SingleInstance();

            _container = builder.Build();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public TopshelfHostService GetService()
        {
            return _container.Resolve<TopshelfHostService>();
        }

        protected abstract void ConfigureContainer(ContainerBuilder builder);
    }
}