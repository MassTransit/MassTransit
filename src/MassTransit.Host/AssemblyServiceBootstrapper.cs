// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Host
{
    using System;
    using Autofac;
    using Configuration;
    using Hosting;
    using Internals.Extensions;
    using Modules;


    /// <summary>
    /// For an assembly that was scanned, this service bootstrapper will configure the service
    /// and endpoint contained in the assembly.
    /// </summary>
    public class AssemblyServiceBootstrapper :
        HostBusServiceBootstrapper
    {
        readonly AssemblyRegistration _registration;

        public AssemblyServiceBootstrapper(ILifetimeScope lifetimeScope, AssemblyRegistration registration)
            : base(lifetimeScope, registration.ServiceSpecificationType)
        {
            _registration = registration;
        }

        protected override void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            base.ConfigureLifetimeScope(builder);

            builder.RegisterType<AssemblyBusServiceConfigurator>()
                .As<IBusServiceConfigurator>();

            builder.RegisterType<FileConfigurationProvider>()
                .WithParameter(TypedParameter.From(_registration.Assembly))
                .As<IConfigurationProvider>()
                .SingleInstance();

            builder.RegisterModule<ConfigurationProviderModule>();

            builder.RegisterAssemblyModules(_registration.Assembly);

            builder.RegisterAssemblyTypes(_registration.Assembly)
                .Where(x => x.HasInterface<IConsumer>())
                .AsSelf();

            builder.RegisterType(_registration.ServiceSpecificationType)
                .AsImplementedInterfaces();

            foreach (var endpointSpecificationType in _registration.EndpointSpecificationTypes)
            {
                builder.RegisterType(endpointSpecificationType)
                    .AsImplementedInterfaces();
            }
        }
    }
}