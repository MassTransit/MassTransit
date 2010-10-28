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
    using Serialization;

    public static class Bus
    {
        static IServiceBus _instance;
        static IEndpointFactory _endpointFactory;

        public static void Initialize(Action<IEndpointFactoryConfigurator> endpointConfig, Action<IServiceBusConfigurator> busConfig, Func<IObjectBuilder> wob)
        {
            var busConfiguration = new BusConfiguration(wob());

            //clear out the instance
            _instance = null;

            var objectBuilder = wob();

            //setup the transport stuff
            
            Action<IEndpointFactoryConfigurator> ecc = ec =>
            {
                endpointConfig(ec);
                ec.SetObjectBuilder(objectBuilder);
            };

            
            _endpointFactory = EndpointFactoryConfigurator.New(ecc);

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

    public interface BusConfiguration
    {
        
    }

    public class XBusConfiguration //what to name?
    {
        EndpointFactoryConfigurator _epc;
        ServiceBusConfigurator _sbc;

        public XBusConfiguration(IObjectBuilder builder)
        {
            _epc = new EndpointFactoryConfigurator();
            _sbc = new ServiceBusConfigurator();

            _epc.SetObjectBuilder(builder);
            _sbc.SetObjectBuilder(builder);
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

        public IServiceBus CreateBus()
        {
          
            return _sbc.CreateServiceBus();
        }
    }


    //extension method per transport
    //
}