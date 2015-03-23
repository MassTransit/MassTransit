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
    using Configurators;
    using PipeBuilders;
    using Pipeline;


    public class DelegatePipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly Action<T> _callback;

        public DelegatePipeSpecification(Action<T> callback)
        {
            _callback = callback;
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new DelegateFilter<T>(_callback));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_callback == null)
                yield return this.Failure("Callback", "must not be null");
        }
    }
}