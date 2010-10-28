// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit
{
    using System;
    using Configuration;

    public static class Bus
    {
        static IServiceBus _instance;

        public static void Initialize(Action<IEndpointFactoryConfigurator> endpointConfig, Action<IServiceBusConfigurator> busConfig, Func<IObjectBuilder> wob)
        {
            //clear out the instance
            _instance = null;

            var objectBuilder = wob();

            //setup the transport stuff
            
            Action<IEndpointFactoryConfigurator> ecc = ec =>
            {
                endpointConfig(ec);
                ec.SetObjectBuilder(objectBuilder);
            };

            
            IEndpointFactory endpointFactory = EndpointFactoryConfigurator.New(ecc);

            ServiceBusConfigurator.Defaults(c => c.SetObjectBuilder(objectBuilder));

            //TODO: Need to allow the setting of the EF here
            Action<IServiceBusConfigurator> bcc = bc =>
            {
                busConfig(bc);
                bc.SetObjectBuilder(objectBuilder);
            };

            _instance = ServiceBusConfigurator.New(busConfig);
        }

        public static IServiceBus Instance()
        {
            return _instance;
        }
    }
}