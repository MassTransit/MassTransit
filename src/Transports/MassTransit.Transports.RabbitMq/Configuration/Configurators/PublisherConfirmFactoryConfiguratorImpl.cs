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
namespace MassTransit.Transports.RabbitMq.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using MassTransit.Configurators;
    using PublisherConfirm;

    public class PublisherConfirmFactoryConfiguratorImpl :
        PublisherConfirmFactoryConfigurator,
        RabbitMqTransportFactoryBuilderConfigurator
    {
        readonly bool _usePublisherConfirms;
        readonly Action<ulong, string> _registerMessageAction;
        readonly Action<ulong, bool> _acktion;
        readonly Action<ulong, bool> _nacktion;

        public PublisherConfirmFactoryConfiguratorImpl(bool usePublisherConfirms, Action<ulong, string> registerMessageAction, 
            Action<ulong, bool> acktion, Action<ulong, bool> nacktion)
        {
            _usePublisherConfirms = usePublisherConfirms;
            _registerMessageAction = registerMessageAction;
            _acktion = acktion;
            _nacktion = nacktion;
        }

        public RabbitMqTransportFactoryBuilder Configure(RabbitMqTransportFactoryBuilder builder)
        {
            builder.SetPublisherConfirmSettings(
                new PublisherConfirmSettings
                    {
                        UsePublisherConfirms = _usePublisherConfirms,
                        RegisterMessageAction = _registerMessageAction,
                        Acktion =  _acktion,
                        Nacktion = _nacktion
                    }
                );

            return builder;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_usePublisherConfirms)
            {
                if (_registerMessageAction == null)
                    yield return this.Failure("RegisterMessageAction", "RegisterMessageAction must be specified if publisher confirms are enabled");
                if (_acktion == null)
                    yield return this.Failure("Acktion", "Acktion must be specified if publisher confirms are enabled");
                if(_nacktion == null)
                    yield return this.Failure("Nacktion", "Nacktion must be specified if publisher confirms are enabled");
            }
        }

    }
}