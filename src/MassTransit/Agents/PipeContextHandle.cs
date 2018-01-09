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
    using System.Threading.Tasks;


    /// <summary>
    /// A handle to a PipeContext instance (of type <typeparam name="TContext">T</typeparam>), which can be disposed
    /// once it is no longer needed (or can no longer be used).
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface PipeContextHandle<TContext> :
        IAsyncDisposable
        where TContext : class, PipeContext
    {
        /// <summary>
        /// True if the context has been disposed (and can no longer be used)
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// The <typeparamref name="TContext"/> context
        /// </summary>
        Task<TContext> Context { get; }
    }
}