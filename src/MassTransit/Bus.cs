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
    using Transports;

    public static class Bus
    {
        static IServiceBus _instance;
        static IEndpointResolver _resolver;

        public static void Initialize(IObjectBuilder builder, Action<BusConfiguration> cfg)
        {
            Reset();

            using (var erc = new EndpointResolverConfigurator())
            {
                erc.AddTransportFactory<LoopbackTransportFactory>();
                erc.AddTransportFactory<MulticastUdpTransportFactory>();

                var sbc = new ServiceBusConfigurator();
                var cbc = new ControlBusConfigurator();

                erc.SetObjectBuilder(builder);
                sbc.SetObjectBuilder(builder);
                cbc.SetObjectBuilder(builder);

                var joint = new JointConfiguration(builder, erc, sbc, cbc);

                cfg(joint);

                _resolver = erc.Create();

                sbc.SetEndpointFactory(_resolver);
                cbc.SetEndpointFactory(_resolver);

                sbc.UseControlBus(cbc.Create());

                _instance = sbc.Create();
            }
        }

        static void Reset()
        {
            if(_instance != null)
                _instance.Dispose();
            if (_resolver != null)
                _resolver.Dispose();


            _instance = null;
            _resolver = null;
        }


        public static IEndpointResolver Resolver()
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