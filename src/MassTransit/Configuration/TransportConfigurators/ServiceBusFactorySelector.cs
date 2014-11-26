// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.TransportConfigurators
{
    using System;
    using System.Collections.Generic;
    using Configurators;


    public class ServiceBusFactorySelector :
        IServiceBusFactorySelector,
        Configurator
    {
        IServiceBusFactory _factory;

        public IEnumerable<ValidationResult> Validate()
        {
            if (_factory == null)
                yield return this.Failure("TransportBuilder", "must be configured");
        }

        public void SetServiceBusFactory(IServiceBusFactory factory)
        {
            _factory = factory;
        }

        public IBusControl Build()
        {
            ConfigurationResult result = ConfigurationResultImpl.CompileResults(_factory.Validate());

            try
            {
                return _factory.CreateBus();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception was thrown during service bus creation", ex);
            }
        }
    }
}