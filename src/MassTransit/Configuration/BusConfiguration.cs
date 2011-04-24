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
    using Configurators;
    using Serialization;

    public interface BusConfiguration
    {
        /// <summary>
        /// Specify the endpoint from which messages should be read
        /// </summary>
        /// <param name="uri">The uri of the endpoint</param>
        void ReceiveFrom(Uri uri);


        /// <summary>
        /// Specify the endpoint to send errors to
        /// </summary>
        /// <param name="uri"></param>
        void SendErrorsTo(Uri uri);

        
        void UseCustomSerializer<TSerializer>() where TSerializer : IMessageSerializer;


        void ConfigureService<TService>(Action<TService> configure) where TService : IBusServiceConfigurator, new();

        void DisableAutoStart();

        void Advanced(Action<AdvancedConfiguration> advCfg);
        ////// advanced settings
        // saga persistors?
        // subscription repo


        void AddTransportFactory(Type transportFactory);


        void CreateMissingQueues();
    }
}