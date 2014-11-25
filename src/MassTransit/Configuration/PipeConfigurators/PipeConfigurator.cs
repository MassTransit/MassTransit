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
namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using Logging;
    using PipeBuilders;
    using Pipeline;


    public class PipeConfigurator<T> :
        IPipeConfigurator<T>,
        Configurator
        where T : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<PipeConfigurator<T>>();

        readonly List<IPipeBuilderConfigurator<T>> _pipeBuilderConfigurators;

        public PipeConfigurator()
        {
            _pipeBuilderConfigurators = new List<IPipeBuilderConfigurator<T>>();
        }

        void IPipeConfigurator<T>.AddPipeBuilderConfigurator(IPipeBuilderConfigurator<T> configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            _pipeBuilderConfigurators.Add(configurator);
        }

        IEnumerable<ValidationResult> Configurator.Validate()
        {
            return _pipeBuilderConfigurators.SelectMany(x => x.Validate());
        }

        void ValidatePipeConfiguration()
        {
            IPipeConfigurationResult result = new PipeConfigurationResult(_pipeBuilderConfigurators.SelectMany(x => x.Validate()));
            if (result.ContainsFailure)
                throw new ConfigurationException(result.GetMessage("The pipe configuration was invalid"));

            if (_log.IsDebugEnabled && result.Any())
                _log.Debug(result.GetMessage("The pipe configuration included messages"));
        }

        public IPipe<T> Build()
        {
            ValidatePipeConfiguration();

            var builder = new PipeBuilder<T>();

            foreach (var configurator in _pipeBuilderConfigurators)
                configurator.Configure(builder);

            return builder.Build();
        }
    }
}