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
    using System.Configuration;
    using Autofac;
    using Configuration;


    public class RabbitMqConfigurationModule :
        Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(GetRabbitMqSettings)
                .As<RabbitMqSettings>()
                .SingleInstance();

            builder.RegisterType<RabbitMqServiceConfigurator>()
                .As<IServiceConfigurator>();
        }

        static RabbitMqSettings GetRabbitMqSettings(IComponentContext context)
        {
            RabbitMqSettings settings;
            if (context.Resolve<ISettingsProvider>().TryGetSettings("RabbitMQ", out settings))
                return settings;

            throw new ConfigurationErrorsException("Unable to resolve RabbitMqSettings from configuration");
        }
    }
}