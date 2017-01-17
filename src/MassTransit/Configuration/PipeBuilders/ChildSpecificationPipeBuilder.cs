// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.PipeBuilders
{
    using GreenPipes;


    public class ChildSpecificationPipeBuilder<T> :
        ISpecificationPipeBuilder<T>
        where T : class, PipeContext
    {
        readonly ISpecificationPipeBuilder<T> _builder;

        public ChildSpecificationPipeBuilder(ISpecificationPipeBuilder<T> builder, bool isImplemented, bool isDelegated)
        {
            _builder = builder;

            IsDelegated = isDelegated;
            IsImplemented = isImplemented;
        }

        public void AddFilter(IFilter<T> filter)
        {
            _builder.AddFilter(filter);
        }

        public bool IsDelegated { get; }

        public bool IsImplemented { get; }

        public ISpecificationPipeBuilder<T> CreateDelegatedBuilder()
        {
            return new ChildSpecificationPipeBuilder<T>(this, IsImplemented, true);
        }

        public ISpecificationPipeBuilder<T> CreateImplementedBuilder()
        {
            return new ChildSpecificationPipeBuilder<T>(this, true, IsDelegated);
        }
    }
}