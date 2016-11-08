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


    public class CompensateActivityLogConfigurator<TActivity, TLog> :
        ICompensateActivityLogConfigurator<TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        readonly IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> _configurator;

        public CompensateActivityLogConfigurator(IPipeConfigurator<CompensateActivityContext<TActivity, TLog>> configurator)
        {
            _configurator = configurator;
        }

        public void AddPipeSpecification(IPipeSpecification<CompensateActivityContext<TLog>> specification)
        {
            _configurator.AddPipeSpecification(new ConsumeContextSpecificationProxy(specification));
        }


        class ConsumeContextSpecificationProxy :
            IPipeSpecification<CompensateActivityContext<TActivity, TLog>>
        {
            readonly IPipeSpecification<CompensateActivityContext<TLog>> _specification;

            public ConsumeContextSpecificationProxy(IPipeSpecification<CompensateActivityContext<TLog>> specification)
            {
                _specification = specification;
            }

            public void Apply(IPipeBuilder<CompensateActivityContext<TActivity, TLog>> builder)
            {
                var messageBuilder = builder as IPipeBuilder<CompensateActivityContext<TLog>>;

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