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
namespace MassTransit.PipeBuilders
{
    using System.Collections.Generic;
    using Pipeline;


    public class PipeBuilder<T> :
        IPipeBuilder<T>
        where T : class, PipeContext
    {
        readonly List<IFilter<T>> _filters;

        public PipeBuilder()
        {
            _filters = new List<IFilter<T>>();
        }

        public void AddFilter(IFilter<T> filter)
        {
            _filters.Add(filter);
        }

        public IPipe<T> Build()
        {
            if (_filters.Count == 0)
                return Pipe.Empty<T>();

            IPipe<T> current = new LastPipe<T>(_filters[_filters.Count - 1]);

            for (int i = _filters.Count - 2; i >= 0; i--)
                current = new FilterPipe<T>(_filters[i], current);

            return current;
        }
    }
}