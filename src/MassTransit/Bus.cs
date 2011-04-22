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
        static IEndpointResolver _resolver;

        public static void Initialize(IObjectBuilder builder, Action<BusConfiguration> cfg)
        {
            if(_instance != null)
                _instance.Dispose();

            _instance = null;

            var busConfig = new MassTransitConfiguration(builder);

            cfg(busConfig);

            _instance = busConfig.CreateBus();
            _resolver = busConfig.GetResolver();
        }


        public static IEndpointResolver Factory()
        {
            if(_instance == null) 
                throw new ConfigurationException("You must call initialize before trying to access the Factory instance.");

            return _resolver;
        }


        public static IServiceBus Instance()
        {
            if(_instance == null) 
                throw new ConfigurationException("You must call initialize before trying to access the Bus instance.");


            return _instance;
        }
    }
}