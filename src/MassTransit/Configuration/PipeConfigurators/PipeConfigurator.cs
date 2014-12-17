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


    public class PipeConfigurator<TContext> :
        IBuildPipeConfigurator<TContext>,
        Configurator
        where TContext : class, PipeContext
    {
        static readonly ILog _log = Logger.Get<PipeConfigurator<TContext>>();

        readonly List<IPipeBuilderConfigurator<TContext>> _configurators;

        public PipeConfigurator()
        {
            _configurators = new List<IPipeBuilderConfigurator<TContext>>();
        }

        /// <summary>
        /// Add middleware to the pipe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configure"></param>
        public void Use<T>(Action<T> configure)
            where T : IPipeConfigurable<TContext>, new()
        {
            var middleware = new T();

            configure(middleware);

            _configurators.Add(middleware);
        }

        /// <summary>
        /// Add middleware to the pipe using the default configuration settings
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        public void Use<T>()
            where T : IPipeConfigurable<TContext>, new()
        {
            _configurators.Add(new T());
        }

        /// <summary>
        /// Add middleware to the pipe using the default configuration settings
        /// </summary>
        /// <typeparam name="T">The context type</typeparam>
        public void Use<T>(Func<T> factoryMethod)
            where T : IPipeConfigurable<TContext>
        {
            _configurators.Add(factoryMethod());
        }

        IEnumerable<ValidationResult> Configurator.Validate()
        {
            return _configurators.SelectMany(x => x.Validate());
        }

        void IPipeConfigurator<TContext>.AddPipeBuilderConfigurator(IPipeBuilderConfigurator<TContext> configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            _configurators.Add(configurator);
        }

        public IPipe<TContext> Build()
        {
            ValidatePipeConfiguration();

            var builder = new PipeBuilder<TContext>();

            foreach (var configurator in _configurators)
                configurator.Build(builder);

            return builder.Build();
        }

        void ValidatePipeConfiguration()
        {
            IPipeConfigurationResult result = new PipeConfigurationResult(_configurators.SelectMany(x => x.Validate()));
            if (result.ContainsFailure)
                throw new ConfigurationException(result.GetMessage("The pipe configuration was invalid"));

            if (_log.IsDebugEnabled && result.Any())
                _log.Debug(result.GetMessage("The pipe configuration included messages"));
        }
    }
}