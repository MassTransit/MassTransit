// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;


    /// <summary>
    /// Joins the output of two source pipes, each of which is only expected to deliver a single context, into a new context
    /// which is delivered to the output pipe.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class JoinPipe<TLeft, TRight, TOutput> :
        Agent,
        IAgent<TOutput>
        where TLeft : class, PipeContext
        where TRight : class, PipeContext
        where TOutput : class, PipeContext
    {
        readonly ISource<TLeft> _left;
        readonly Func<TLeft, TRight, Task<TOutput>> _outputContextFactory;
        readonly ISource<TRight> _right;
        readonly IPipe<TLeft> _leftPipe;
        readonly IPipe<TRight> _rightPipe;

        public JoinPipe(ISource<TLeft> left, ISource<TRight> right, IPipe<TLeft> leftPipe, IPipe<TRight> rightPipe, Func<TLeft, TRight, Task<TOutput>> outputContextFactory)
        {
            _left = left;
            _right = right;
            _leftPipe = leftPipe;
            _rightPipe = rightPipe;
            _outputContextFactory = outputContextFactory;
        }

        public async Task Send(IPipe<TOutput> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            IAsyncPipeContextAgent<TLeft> leftAgent = new AsyncPipeContextAgent<TLeft>();
            IAsyncPipeContextAgent<TRight> rightAgent = new AsyncPipeContextAgent<TRight>();

            try
            {
                var leftPipe = new AsyncPipeContextPipe<TLeft>(leftAgent, _leftPipe);
                var leftTask = _left.Send(leftPipe, cancellationToken);

                var rightPipe = new AsyncPipeContextPipe<TRight>(rightAgent, _rightPipe);
                var rightTask = _right.Send(rightPipe, cancellationToken);

                async Task Join()
                {
                    var leftContext = await leftAgent.Context.ConfigureAwait(false);
                    var rightContext = await rightAgent.Context.ConfigureAwait(false);

                    var outputContext = await _outputContextFactory(leftContext, rightContext).ConfigureAwait(false);

                    await pipe.Send(outputContext).ConfigureAwait(false);
                }

                await Task.WhenAll(leftTask, rightTask, Join()).ConfigureAwait(false);
            }
            finally
            {
                await Task.WhenAll(leftAgent.Stop("Complete", cancellationToken), rightAgent.Stop("Complete", cancellationToken)).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("joinPipe");
        }
    }
}