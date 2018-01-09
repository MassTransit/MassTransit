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


    public static class SupervisorExtensions
    {
        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <param name="context">The context</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IPipeContextAgent<T> AddContext<T>(this ISupervisor supervisor, T context)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            IPipeContextAgent<T> contextAgent = new PipeContextAgent<T>(context);

            supervisor.Add(contextAgent);

            return contextAgent;
        }

        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <param name="context">The context</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IPipeContextAgent<T> AddContext<T>(this ISupervisor supervisor, Task<T> context)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            IPipeContextAgent<T> contextAgent = new PipeContextAgent<T>(context);

            supervisor.Add(contextAgent);

            return contextAgent;
        }

        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <param name="contextHandle">The actual context handle</param>
        /// <param name="context">The active context</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IActivePipeContextAgent<T> AddActiveContext<T>(this ISupervisor supervisor, PipeContextHandle<T> contextHandle, Task<T> context)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var activeContext = new ActivePipeContext<T>(contextHandle, context);

            var contextAgent = new ActivePipeContextAgent<T>(activeContext);

            supervisor.Add(contextAgent);

            return contextAgent;
        }
        
        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <param name="contextHandle">The actual context handle</param>
        /// <param name="context">The active context</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static ActivePipeContextHandle<T> AddActiveContext<T>(this ISupervisor supervisor, PipeContextHandle<T> contextHandle, T context)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var activeContext = new ActivePipeContext<T>(contextHandle, context);

            var contextAgent = new ActivePipeContextAgent<T>(activeContext);

            supervisor.Add(contextAgent);

            return contextAgent;
        }

        /// <summary>
        /// Adds a context to the supervisor as an agent, which can be stopped by the supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor</param>
        /// <typeparam name="T">The context type</typeparam>
        /// <returns>A context handle</returns>
        public static IAsyncPipeContextAgent<T> AddAsyncContext<T>(this ISupervisor supervisor)
            where T : class, PipeContext
        {
            if (supervisor == null)
                throw new ArgumentNullException(nameof(supervisor));

            IAsyncPipeContextAgent<T> contextAgent = new AsyncPipeContextAgent<T>();

            supervisor.Add(contextAgent);

            return contextAgent;
        }
    }
}