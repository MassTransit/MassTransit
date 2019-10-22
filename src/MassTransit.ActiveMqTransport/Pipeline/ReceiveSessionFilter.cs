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
namespace MassTransit.ActiveMqTransport.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;


    /// <summary>
    /// Creates a receiving model context using the connection
    /// </summary>
    public class ReceiveSessionFilter :
        IFilter<ConnectionContext>
    {
        readonly IActiveMqHost _host;
        readonly IPipe<SessionContext> _pipe;

        public ReceiveSessionFilter(IPipe<SessionContext> pipe, IActiveMqHost host)
        {
            _pipe = pipe;
            _host = host;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("receiveSession");

            _pipe.Probe(scope);
        }

        async Task IFilter<ConnectionContext>.Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            var session = await context.CreateSession(context.CancellationToken).ConfigureAwait(false);

            var sessionContext = new ActiveMqSessionContext(context, session, context.CancellationToken);

            void HandleException(Exception exception)
            {
                var disposeAsync = sessionContext.DisposeAsync(CancellationToken.None);
            }

            context.Connection.ExceptionListener += HandleException;

            try
            {
                await _pipe.Send(sessionContext).ConfigureAwait(false);
            }
            finally
            {
                context.Connection.ExceptionListener -= HandleException;

                await sessionContext.DisposeAsync(CancellationToken.None).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
