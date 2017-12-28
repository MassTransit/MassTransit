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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Observables;


    /// <summary>
    /// Converts an inbound context type to a pipe context type post-dispatch
    /// </summary>
    /// <typeparam name="TOutput">The subsequent pipe context type</typeparam>
    public class MessageSendPipe<TOutput> :
        IMessageSendPipe<TOutput>
        where TOutput : class
    {
        readonly SendMessageObservable<TOutput> _observers;
        readonly IPipe<SendContext<TOutput>> _outputPipe;

        public MessageSendPipe(IPipe<SendContext<TOutput>> outputPipe)
        {
            _outputPipe = outputPipe;

            _observers = new SendMessageObservable<TOutput>();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("dispatchPipe");
            scope.Add("outputType", TypeCache<TOutput>.ShortName);

            _outputPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IPipe<SendContext<TOutput>>.Send(SendContext<TOutput> context)
        {
            {
                if (_observers.Count > 0)
                    await _observers.PreSend(context).ConfigureAwait(false);
                try
                {
                    await _outputPipe.Send(context).ConfigureAwait(false);

                    if (_observers.Count > 0)
                        await _observers.PostSend(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (_observers.Count > 0)
                    {
                        await _observers.SendFault(context, ex).ConfigureAwait(false);
                    }

                    throw;
                }
            }
        }

        public ConnectHandle ConnectSendMessageObserver(ISendMessageObserver<TOutput> observer)
        {
            return _observers.Connect(observer);
        }
    }
}