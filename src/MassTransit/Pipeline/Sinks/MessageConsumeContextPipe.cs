// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using Util;


    /// <summary>
    /// Converts a ConsumeContext to a message type and passes the context to 
    /// the output pipe. Supports interception by message type.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class MessageConsumeContextPipe<TMessage> :
        IConsumeContextPipe,
        IConnectPipe,
        IConnectMessageInterceptor
        where TMessage : class
    {
        readonly MessageInterceptorConnectable<TMessage> _connections;
        readonly TeeConsumeContextPipe<TMessage> _output;

        public MessageConsumeContextPipe()
        {
            _output = new TeeConsumeContextPipe<TMessage>();
            _connections = new MessageInterceptorConnectable<TMessage>();
        }

        ConnectHandle IConnectMessageInterceptor.Connect<T>(IMessageInterceptor<T> interceptor)
        {
            var self = _connections as MessageInterceptorConnectable<T>;
            if (self == null)
                throw new InvalidOperationException("The connection type is invalid: " + TypeMetadataCache<T>.ShortName);

            return self.Connect(interceptor);
        }

        ConnectHandle IConnectPipe.Connect<T>(IConsumeContextPipe<T> pipe)
        {
            var output = _output as IConnectPipe<T>;
            if (output == null)
                throw new ArgumentException("Invalid pipe type specified: " + TypeMetadataCache<T>.ShortName);

            return output.Connect(pipe);
        }

        async Task IConsumeContextPipe.Send(ConsumeContext context)
        {
            ConsumeContext<TMessage> consumeContext;
            if (context.TryGetMessage(out consumeContext))
            {
                if(_connections.Count > 0)  
                    await _connections.PreDispatch(consumeContext);

                Exception exception = null;
                try
                {
                    await _output.Send(consumeContext);

                    if (_connections.Count > 0)
                        await _connections.PostDispatch(consumeContext);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (exception != null)
                {
                    if (_connections.Count > 0)
                        await _connections.DispatchFaulted(consumeContext, exception);

                    throw new MessageException(typeof(TMessage), "The message consumer faulted", exception);
                }
            }
        }

        public bool Inspect(IConsumeContextPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _output.Inspect(x));
        }
    }
}