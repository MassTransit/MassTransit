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
using MassTransit.Exceptions;

namespace MassTransit.Configuration
{
    using System;
    using Serialization;
    using Transports;

    public class MassTransitConfiguration //what to name?
        : BusConfiguration
    {
        readonly EndpointResolverConfigurator _epc;
        readonly ServiceBusConfigurator _sbc;
        private bool _serializerHasBeenSet;
        private IEndpointResolver _resolver;

        public MassTransitConfiguration(IObjectBuilder builder)
        {
            _epc = new EndpointResolverConfigurator();
            _sbc = new ServiceBusConfigurator();

            _epc.SetObjectBuilder(builder);
            _sbc.SetObjectBuilder(builder);

            _epc.AddTransportFactory<LoopbackTransportFactory>();
            _epc.AddTransportFactory<MulticastUdpTransportFactory>();
        }

        public void AddTransportFactory(Type transportFactoryType)
        {
            _epc.AddTransportFactory(transportFactoryType);
        }

        public void ReceiveFrom(Uri uri)
        {
            _sbc.ReceiveFrom(uri);
        }

        public void SendErrorsTo(Uri uri)
        {
            _sbc.SendErrorsTo(uri);
        }

        public void UseCustomSerializer<TSerializer>() where TSerializer : IMessageSerializer
        {
            if(!_serializerHasBeenSet)
                throw new ConfigurationException("You have already choosen a serializer beyond the default one.");

            _serializerHasBeenSet = true;
            _epc.SetDefaultSerializer<TSerializer>();
        }


        //effectively internal
        public IServiceBus CreateBus()
        {
            //need to pass the epf into the sbc
            _resolver = _epc.Create();
            _sbc.SetEndpointFactory(_epc.Create());

            //TODO: Control Bus needs a concurrent receiver of 1

            return _sbc.Create();
        }

        public IEndpointResolver GetResolver()
        {
            if (_resolver != null)
                return _resolver;

            throw new ConfigurationException("You need to initialize MassTransit first 'Use Bus.Initialize' ");
        }

        public void ConfigureService<TService>(Action<TService> configure) where TService : IServiceConfigurator, new()
        {
            _sbc.ConfigureService(configure);
        }

        public void Advanced(Action<AdvancedConfiguration> advCfg)
        {
            var adv = new CrazyAdvancedShit(_epc, _sbc);
            advCfg(adv);
        }

        public void DisableAutoStart()
        {
            _sbc.DisableAutoStart();
        }

        public void EnableAutoSubscribe()
        {
            _sbc.EnableAutoSubscribe();
        }
    }
}