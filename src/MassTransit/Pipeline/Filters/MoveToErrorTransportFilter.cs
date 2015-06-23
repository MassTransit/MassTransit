// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using Transports;
    using Util;


    /// <summary>
    /// Moves a message received to an error transport without any deserialization
    /// </summary>
    public class MoveToErrorTransportFilter :
        IFilter<ReceiveContext>
    {
        readonly Func<Task<ISendTransport>> _getErrorTransport;
        readonly ILog _log = Logger.Get<MoveToErrorTransportFilter>();

        public MoveToErrorTransportFilter(Func<Task<ISendTransport>> getErrorTransport)
        {
            _getErrorTransport = getErrorTransport;
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("moveToErrorTransport");
        }

        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            ISendTransport transport = await _getErrorTransport();

            IPipe<SendContext> pipe = Pipe.Execute<SendContext>(sendContext =>
            {
//                if (exception != null)
//                {
                // if exception is AggregateException, get InnerException
//                    moveContext.Headers.Set("MT-Fault-Message", exception.Message);
//                    moveContext.Headers.Set("MT-Fault-StackTrace", exception.StackTrace);
//                }

                sendContext.Headers.Set("MT-Error-MachineName", HostMetadataCache.Host.MachineName);
                sendContext.Headers.Set("MT-Error-ProcessName", HostMetadataCache.Host.ProcessName);
                sendContext.Headers.Set("MT-Error-Assembly", HostMetadataCache.Host.Assembly);
                sendContext.Headers.Set("MT-Error-AssemblyVersion", HostMetadataCache.Host.AssemblyVersion);
            });

            await transport.Move(context, pipe);

            await next.Send(context);
        }
    }
}