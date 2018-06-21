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
    using Context;
    using GreenPipes;
    using GreenPipes.Filters;
    using GreenPipes.Specifications;
    using Saga;


    public class ConsumerConsumeContextRescuePipeSpecification<T> :
        ExceptionSpecification,
        IRescueConfigurator,
        IPipeSpecification<ConsumerConsumeContext<T>>
        where T : class, ISaga
    {
        readonly IPipe<ExceptionConsumerConsumeContext<T>> _rescuePipe;

        public ConsumerConsumeContextRescuePipeSpecification(IPipe<ExceptionConsumerConsumeContext<T>> rescuePipe)
        {
            _rescuePipe = rescuePipe;
        }

        public void Apply(IPipeBuilder<ConsumerConsumeContext<T>> builder)
        {
            builder.AddFilter(new RescueFilter<ConsumerConsumeContext<T>, ExceptionConsumerConsumeContext<T>>(_rescuePipe, Filter,
                (context, ex) => new RescueExceptionConsumerConsumeContext<T>(context, ex)));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rescuePipe == null)
                yield return this.Failure("RescuePipe", "must not be null");
        }
    }
}