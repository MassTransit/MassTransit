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
    using Transports;


    /// <summary>
    /// Moves a message received to an error transport without any deserialization
    /// </summary>
    public class MoveExceptionToTransportFilter :
        IFilter<ExceptionReceiveContext>
    {
        readonly Uri _destinationAddress;
        readonly Func<Task<ISendTransport>> _getDestinationTransport;

        public MoveExceptionToTransportFilter(Uri destinationAddress, Func<Task<ISendTransport>> getDestinationTransport)
        {
            _getDestinationTransport = getDestinationTransport;
            _destinationAddress = destinationAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("moveFault");
            scope.Add("destinationAddress", _destinationAddress);
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            ISendTransport transport = await _getDestinationTransport();

            IPipe<SendContext> pipe = Pipe.Execute<SendContext>(sendContext =>
            {
                sendContext.Headers.Set("MT-Reason", "fault");

                Exception exception = context.Exception.GetBaseException();

                sendContext.Headers.Set("MT-Fault-Message", exception.Message);
                sendContext.Headers.Set("MT-Fault-StackTrace", exception.StackTrace);

                sendContext.SetHostHeaders();
            });

            await transport.Move(context, pipe);

            await next.Send(context);
        }
    }
}