// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.BusConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using Logging;
    using Util;

    public class ControlBusConfiguratorImpl :
        ControlBusConfigurator,
        BusBuilderConfigurator
    {
        static readonly ILog _log = Logger.Get(typeof(ControlBusConfiguratorImpl));

        readonly IList<BusBuilderConfigurator> _configurators;
        Uri _uri;

        public ControlBusConfiguratorImpl()
        {
            _configurators = new List<BusBuilderConfigurator>();
        }

        public BusBuilder Configure(BusBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");

            builder.Match<ServiceBusBuilder>(x =>
                {
                    var settings = new ServiceBusSettings(builder.Settings);

                    settings.InputAddress = _uri ?? builder.Settings.InputAddress.AppendToPath("_control");

                    // the endpoint factory is already created, so we can't set the endpoint here
                    // we really need this to be part of another step, but i don't have a clue how yet.
                    //_configurator.ConfigureEndpoint(_uri, x => x.PurgeExistingMessages());

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Configuring control bus for {0} at {1}", builder.Settings.InputAddress,
                            settings.InputAddress);

                    settings.ConcurrentConsumerLimit = 1;
                    settings.ConcurrentReceiverLimit = 1;
                    settings.AutoStart = true;

                    BusBuilder controlBusBuilder = new ControlBusBuilderImpl(settings);

                    controlBusBuilder = _configurators
                        .Aggregate(controlBusBuilder, (current, configurator) => configurator.Configure(current));

                    IControlBus controlBus = controlBusBuilder.Build();

                    x.UseControlBus(controlBus);
                });

            return builder;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return from configurator in _configurators
                   from result in configurator.Validate()
                   select result.WithParentKey("ControlBus");
        }

        public void ReceiveFrom(Uri uri)
        {
            _uri = uri;
        }
    }
}