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
namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Transports;

    /// <summary>
    /// Moves a message received to an error transport without any deserialization
    /// </summary>
    public class MoveToErrorTransportFilter :
        IFilter<ReceiveContext>
    {
        readonly Task<ISendTransport> _errorTransport;

        public MoveToErrorTransportFilter(Task<ISendTransport> errorTransport)
        {
            _errorTransport = errorTransport;
        }

        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            ISendTransport transport = await _errorTransport;

            await transport.Move(context);

            await next.Send(context);
        }

        bool IFilter<ReceiveContext>.Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }
    }
}