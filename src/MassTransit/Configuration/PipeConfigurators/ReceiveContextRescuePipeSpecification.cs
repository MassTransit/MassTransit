// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Configurators;
    using PipeBuilders;
    using Pipeline;
    using Pipeline.Filters;
    using Policies;


    public class ReceiveContextRescuePipeSpecification :
        IPipeSpecification<ReceiveContext>
    {
        readonly IPolicyExceptionFilter _exceptionFilter;
        readonly IPipe<ExceptionReceiveContext> _rescuePipe;

        public ReceiveContextRescuePipeSpecification(IPipe<ExceptionReceiveContext> rescuePipe, IPolicyExceptionFilter exceptionFilter)
        {
            _rescuePipe = rescuePipe;
            _exceptionFilter = exceptionFilter;
        }

        public void Apply(IPipeBuilder<ReceiveContext> builder)
        {
            builder.AddFilter(new RescueReceiveContextFilter<ReceiveContext>(_rescuePipe, _exceptionFilter ?? new AllPolicyExceptionFilter()));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rescuePipe == null)
                yield return this.Failure("RescuePipe", "must not be null");
        }
    }
}