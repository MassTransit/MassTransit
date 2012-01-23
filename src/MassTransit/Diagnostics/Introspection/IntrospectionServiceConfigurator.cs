// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Diagnostics.Introspection
{
    using System;
    using BusServiceConfigurators;

    public class IntrospectionServiceConfigurator :
        BusServiceConfigurator
    {
        public Type ServiceType
        {
            get { return typeof (IntrospectionBusService); }
        }

        public BusServiceLayer Layer
        {
            get { return BusServiceLayer.Application; }
        }

        public IBusService Create(IServiceBus bus)
        {
            return new IntrospectionBusService();
        }
    }
}