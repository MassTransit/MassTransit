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
namespace MassTransit.Configuration
{
    using System;
    using Exceptions;
    using Serialization;
    using Transports;

    public class JointConfiguration
        : BusConfiguration
    {
        readonly EndpointResolverConfigurator _epc;
        readonly ServiceBusConfigurator _sbc;
        readonly ControlBusConfigurator _cbc;
        bool _serializerHasBeenSet;

        public JointConfiguration(IObjectBuilder builder, EndpointResolverConfigurator epc, ServiceBusConfigurator sbc, ControlBusConfigurator cbc)
        {
            _epc = epc;
            _sbc = sbc;
            _cbc = cbc;


        }

        public void AddTransportFactory(Type transportFactoryType)
        {
            _epc.AddTransportFactory(transportFactoryType);
        }

        public void CreateMissingQueues()
        {
            EndpointConfigurator.Defaults(x => x.CreateMissingQueues = true);
        }

        public void ReceiveFrom(Uri uri)
        {
            _sbc.ReceiveFrom(uri);
            _cbc.ReceiveFrom(uri.AppendToPath("_control"));
        }

        public void SendErrorsTo(Uri uri)
        {
            _sbc.SendErrorsTo(uri);
            _cbc.SendErrorsTo(uri);
        }

        public void UseCustomSerializer<TSerializer>() where TSerializer : IMessageSerializer
        {
            if (!_serializerHasBeenSet)
                throw new ConfigurationException("You have already choosen a serializer beyond the default one.");

            _serializerHasBeenSet = true;
            _epc.SetDefaultSerializer<TSerializer>();
        }

        public void ConfigureService<TService>(Action<TService> configure) where TService : IServiceConfigurator, new()
        {
            _sbc.ConfigureService(configure);
        }

        public void Advanced(Action<AdvancedConfiguration> advCfg)
        {
            var adv = new AdvancedConfigurationOptions(_epc, _sbc);
            advCfg(adv);
        }

        public void DisableAutoStart()
        {
            _sbc.DisableAutoStart();
            _cbc.DisableAutoStart();
        }
    }
}