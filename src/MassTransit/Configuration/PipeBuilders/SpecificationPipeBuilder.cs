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
    using System.Collections.Generic;
    using GreenPipes;
    using GreenPipes.Pipes;


    public class SpecificationPipeBuilder<TContext> :
        IBuildSpecificationPipeBuilder<TContext>
        where TContext : class, PipeContext
    {
        readonly List<IFilter<TContext>> _filters;

        public SpecificationPipeBuilder()
        {
            _filters = new List<IFilter<TContext>>();
        }

        public void AddFilter(IFilter<TContext> filter)
        {
            _filters.Add(filter);
        }

        public IPipe<TContext> Build()
        {
            if (_filters.Count == 0)
                return Pipe.Empty<TContext>();

            IPipe<TContext> current = new LastPipe<TContext>(_filters[_filters.Count - 1]);

            for (var i = _filters.Count - 2; i >= 0; i--)
                current = new FilterPipe<TContext>(_filters[i], current);

            return current;
        }

        public IPipe<TContext> Build(IPipe<TContext> lastPipe)
        {
            if (_filters.Count == 0)
                return lastPipe;

            IPipe<TContext> current = lastPipe;

            for (var i = _filters.Count - 1; i >= 0; i--)
                current = new FilterPipe<TContext>(_filters[i], current);

            return current;
        }

        public bool IsDelegated => false;
        public bool IsImplemented => false;

        public ISpecificationPipeBuilder<TContext> CreateDelegatedBuilder()
        {
            return new ChildSpecificationPipeBuilder<TContext>(this, IsImplemented, true);
        }

        public ISpecificationPipeBuilder<TContext> CreateImplementedBuilder()
        {
            return new ChildSpecificationPipeBuilder<TContext>(this, true, IsDelegated);
        }
    }
}