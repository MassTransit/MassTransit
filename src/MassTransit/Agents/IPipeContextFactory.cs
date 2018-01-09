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
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Used to create the actual context, and the active context usages
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    public interface IPipeContextFactory<TContext>
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Create the pipe context, which is the actual context, and not a copy of it
        /// </summary>
        /// <param name="supervisor">The supervisor containing the context</param>
        /// <returns>A handle to the pipe context</returns>
        PipeContextHandle<TContext> CreateContext(ISupervisor supervisor);

        /// <summary>
        /// Create an active pipe context, which is a reference to the actual context
        /// </summary>
        /// <param name="supervisor">The supervisor containing the context</param>
        /// <param name="context">The actual context</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> use for the active context</param>
        /// <returns>A handle to the active context</returns>
        ActivePipeContextHandle<TContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<TContext> context,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}