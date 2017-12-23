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
    /// <typeparam name="TMessage">The subsequent pipe context type</typeparam>
    public class MessagePublishPipe<TMessage> :
        IMessagePublishPipe<TMessage>
        where TMessage : class
    {
        readonly PublishMessageObservable<TMessage> _observers;
        readonly IPipe<PublishContext<TMessage>> _outputPipe;

        public MessagePublishPipe(IPipe<PublishContext<TMessage>> outputPipe)
        {
            _outputPipe = outputPipe;

            _observers = new PublishMessageObservable<TMessage>();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("messagePublishPipe");
            scope.Add("messageType", TypeCache<TMessage>.ShortName);

            _outputPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IPipe<PublishContext<TMessage>>.Send(PublishContext<TMessage> context)
        {
            {
                if (_observers.Count > 0)
                    await _observers.PrePublish(context).ConfigureAwait(false);
                try
                {
                    await _outputPipe.Send(context).ConfigureAwait(false);

                    if (_observers.Count > 0)
                        await _observers.PostPublish(context).ConfigureAwait(false);

                    await _outputPipe.Send(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (_observers.Count > 0)
                    {
                        await _observers.PublishFault(context, ex).ConfigureAwait(false);
                    }

                    throw;
                }
            }
        }

        public ConnectHandle ConnectPublishMessageObserver(IPublishMessageObserver<TMessage> observer)
        {
            return _observers.Connect(observer);
        }
    }
}