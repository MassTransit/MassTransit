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
namespace MassTransit.Topology
{
    using GreenPipes;


    /// <summary>
    /// A pipe builder used by topologies, which indicates whether the message type
    /// is either delegated (called from a sub-specification) or implemented (being called
    /// when the actual type is a subtype and this is an implemented type).
    /// </summary>
    /// <typeparam name="T">The pipe context type</typeparam>
    public interface ITopologyPipeBuilder<T> :
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

        /// <summary>
        /// Creates a new builder where the Delegated flag is true
        /// </summary>
        /// <returns></returns>
        ITopologyPipeBuilder<T> CreateDelegatedBuilder();
    }
}