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
    using Exceptions;

    public static class Bus
    {
        static IServiceBus _instance;
        static IEndpointFactory _factory;

        public static void Initialize(IObjectBuilder builder, Action<BusConfiguration, IEndpointFactory> cfg, params Type[] transports)
        {
            if(_instance != null)
                _instance.Dispose();

            _instance = null;

            _factory = EndpointFactoryConfigurator.New(e =>
            {
                foreach (var transport in transports)
                    e.RegisterTransport(transport);

            });

            var busConfig = new MassTransitConfiguration(builder, _factory);
            cfg(busConfig, _factory);

            _instance = busConfig.CreateBus();
        }

        public static IEndpointFactory Factory()
        {
            if(_instance == null) 
                throw new ConfigurationException("You must call initialize before trying to access the Factory instance.");
            return _factory;
        }

        public static IServiceBus Instance()
        {
            if(_instance == null) 
                throw new ConfigurationException("You must call initialize before trying to access the Bus instance.");


            return _instance;
        }
    }
}