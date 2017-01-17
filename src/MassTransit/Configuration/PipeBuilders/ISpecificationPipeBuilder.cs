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


    public interface ISpecificationPipeBuilder<T> :
        IPipeBuilder<T>
        where T : class, PipeContext
    {
        /// <summary>
        /// If true, this is a delegated builder, and implemented message types
        /// and/or topology items should not be applied
        /// </summary>
        bool IsDelegated { get; }

        /// <summary>
        /// If true, this is a builder for implemented types, so don't go down
        /// the rabbit hole twice.
        /// </summary>
        bool IsImplemented { get; }


        ISpecificationPipeBuilder<T> CreateDelegatedBuilder();


        ISpecificationPipeBuilder<T> CreateImplementedBuilder();
    }
}