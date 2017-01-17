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
namespace MassTransit.Tests
{
    using GreenPipes;


    public interface IConsumePipeBuilder :
        IPipeBuilder<ConsumeContext>
    {
        /// <summary>
        /// Add a filter to the pipe after any existing filters
        /// </summary>
        /// <param name="filter">The filter to add</param>
        void AddFilter<T>(IFilter<ConsumeContext<T>> filter)
            where T : class;
    }
}