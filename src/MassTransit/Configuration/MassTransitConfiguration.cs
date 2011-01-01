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
namespace MassTransit.Configuration
{
    using System;
    using Serialization;
    using Transports;

    public class MassTransitConfiguration //what to name?
        : BusConfiguration
    {
        readonly EndpointFactoryConfigurator _epc;
        readonly ServiceBusConfigurator _sbc;
        readonly IEndpointResolver _resolver;
        public MassTransitConfiguration(IObjectBuilder builder, IEndpointResolver resolver)
        {
            _epc = new EndpointFactoryConfigurator();
            _sbc = new ServiceBusConfigurator();

            _epc.SetObjectBuilder(builder);
            _sbc.SetObjectBuilder(builder);

            _epc.RegisterTransport<LoopbackEndpoint>();
            _epc.RegisterTransport<MulticastUdpEndpoint>();
            _resolver = resolver;
        }

        public void RegisterTransport(Type transportType)
        {
            _epc.RegisterTransport(transportType);
        }

        public void RegisterTransport<TTransport>() where TTransport : IEndpoint
        {
            _epc.RegisterTransport<TTransport>();
        }

        public void ReceiveFrom(string uriString)
        {
            _sbc.ReceiveFrom(uriString);
        }

        public void ReceiveFrom(Uri uri)
        {
            _sbc.ReceiveFrom(uri);
        }

        public void UseDotNetXmlSerilaizer()
        {
            UseCustomSerializer<DotNotXmlMessageSerializer>();
        }

        public void UseJsonSerializer()
        {
            UseCustomSerializer<JsonMessageSerializer>();
        }

        public void UseXmlSerializer()
        {
            UseCustomSerializer<XmlMessageSerializer>();
        }

        public void UseBinarySerializer()
        {
            UseCustomSerializer<BinaryMessageSerializer>();
        }

        public void UseCustomSerializer<TSerializer>() where TSerializer : IMessageSerializer
        {
            _epc.SetDefaultSerializer<TSerializer>();
        }

        public void SendErrorsTo(string uriString)
        {
            _sbc.SendErrorsTo(uriString);
        }

        public void SendErrorsTo(Uri uri)
        {
            _sbc.SendErrorsTo(uri);
        }


        public IServiceBus CreateBus()
        {
            //need to pass the epf into the sbc
            _sbc.SetEndpointFactory(_resolver);

            //TODO: Control Bus needs a concurrent receiver of 1

            return _sbc.Create();
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