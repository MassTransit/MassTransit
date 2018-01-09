// Copyright 2012-2018 Chris Patterson
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
namespace GreenPipes.Agents
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// An active, in-use reference to a pipe context.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ActivePipeContextHandle<TContext> :
        PipeContextHandle<TContext>
        where TContext : class, PipeContext
    {
        /// <summary>
        /// If the use of this context results in a fault which should cause the context to be disposed, this method signals that behavior to occur.
        /// </summary>
        /// <param name="exception">The bad thing that happened</param>
        Task Faulted(Exception exception);
    }
}