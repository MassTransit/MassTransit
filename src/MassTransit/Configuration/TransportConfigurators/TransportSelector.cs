// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.TransportConfigurators
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Exceptions;


    public class TransportSelector :
        ITransportSelector,
        Configurator

    {
        ITransportBuilder _builder;

        public IEnumerable<ValidationResult> Validate()
        {
            if (_builder == null)
                yield return this.Failure("TransportBuilder", "must be configured");
        }

        public void SelectTransport(ITransportBuilder builder)
        {
            _builder = builder;
        }

        public IServiceBus Build()
        {
            ConfigurationResult result = ConfigurationResultImpl.CompileResults(_builder.Validate());

            try
            {
                return _builder.Build();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception was thrown during service bus creation", ex);
            }
        }
    }
}