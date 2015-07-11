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
namespace MassTransit.ConsumeConfigurators
{
    using System.Collections.Generic;
    using Configurators;
    using ConsumeConnectors;
    using Internals.Extensions;


    public class InstanceConfigurator :
        IInstanceConfigurator,
        IReceiveEndpointSpecification
    {
        readonly object _instance;

        public InstanceConfigurator(object instance)
        {
            _instance = instance;
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            InstanceConnectorCache.GetInstanceConnector(_instance.GetType()).ConnectInstance(builder, _instance);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_instance == null)
                yield return this.Failure("The instance cannot be null. This should have come in the ctor.");

            if (_instance != null && !_instance.GetType().HasInterface<IConsumer>())
            {
                yield return this.Warning(string.Format("The instance of {0} does not implement any consumer interfaces",
                    _instance.GetType().GetTypeName()));
            }
        }
    }
}