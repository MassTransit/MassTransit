// Copyright 2007-2011 The Apache Software Foundation.
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
    using System.Linq;
    using Autofac;
    using BusConfigurators;
    using Magnum.Extensions;

    public static class MassTransitAutofacExtensions
    {
        public static UnsubscribeAction LoadConsumersFromContainer(this ServiceBusConfigurator config, IContainer container)
        {
            var concretes = from r in container.ComponentRegistry.Registrations
                            where r.Services.Any(s => s.Implements<IConsumer>())
                            select r.Target;

            //concretes.Each(config.RegisterSubscription);

            return () => true;
        }
    }
}