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
namespace GreenPipes
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A source provides the context which is sent to the specified pipe.
    /// </summary>
    /// <typeparam name="TContext">The pipe context type</typeparam>
    public interface IPipeContextSource<out TContext> :
        IProbeSite
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Send a context from the source through the specified pipe
        /// </summary>
        /// <param name="pipe">The destination pipe</param>
        /// <param name="cancellationToken">The cancellationToken, which should be included in the context</param>
        Task Send(IPipe<TContext> pipe, CancellationToken cancellationToken = default(CancellationToken));
    }


    /// <summary>
    /// A source which provides the context using the input context to select the appropriate source.
    /// </summary>
    /// <typeparam name="TContext">The output context type</typeparam>
    /// <typeparam name="TInput">The input context type</typeparam>
    public interface ISource<out TContext, in TInput> :
        IProbeSite
        where TContext : class, PipeContext
        where TInput : class, PipeContext
    {
        /// <summary>
        /// Send a context from the source through the specified pipe, using the input context to select the proper source.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        Task Send(TInput context, IPipe<TContext> pipe);
    }
}