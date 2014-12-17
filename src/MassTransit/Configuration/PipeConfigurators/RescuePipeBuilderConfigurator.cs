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
    using System.Collections.Generic;
    using Configurators;
    using PipeBuilders;
    using Pipeline;
    using Pipeline.Filters;


    public class RescuePipeBuilderConfigurator<T> :
        IPipeBuilderConfigurator<T>
        where T : class, PipeContext
    {
        readonly RescueExceptionFilter _exceptionFilter;
        readonly IPipe<T> _rescuePipe;

        public RescuePipeBuilderConfigurator(IPipe<T> rescuePipe, RescueExceptionFilter exceptionFilter)
        {
            _rescuePipe = rescuePipe;
            _exceptionFilter = exceptionFilter;
        }

        public void Build(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new RescueFilter<T>(_rescuePipe, _exceptionFilter));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_exceptionFilter == null)
                yield return this.Failure("ExceptionFilter", "must not be null");
            if (_rescuePipe == null)
                yield return this.Failure("Pipe", "must not be null");
        }
    }
}