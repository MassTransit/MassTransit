// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using Courier;
    using GreenPipes;


    public class ExecuteActivityArgumentsConfigurator<TActivity, TArguments> :
        IExecuteActivityArgumentsConfigurator<TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> _configurator;

        public ExecuteActivityArgumentsConfigurator(IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<ExecuteActivityContext<TArguments>> specification)
        {
            _configurator.AddPipeSpecification(new ConsumeContextSpecificationProxy(specification));
        }


        class ConsumeContextSpecificationProxy :
            IPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>
        {
            readonly IPipeSpecification<ExecuteActivityContext<TArguments>> _specification;

            public ConsumeContextSpecificationProxy(IPipeSpecification<ExecuteActivityContext<TArguments>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<ExecuteActivityContext<TActivity, TArguments>> builder)
            {
                var messageBuilder = builder as IPipeBuilder<ExecuteActivityContext<TArguments>>;

                if (messageBuilder != null)
                    _specification.Apply(messageBuilder);
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }
        }
    }
}