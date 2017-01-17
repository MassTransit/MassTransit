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
namespace MassTransit.Topology.Filters
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    /// <summary>
    /// Sets the CorrelationId header uses the supplied implementation.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class SetCorrelationIdFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly ISetCorrelationId<T> _setCorrelationId;

        public SetCorrelationIdFilter(ISetCorrelationId<T> setCorrelationId)
        {
            _setCorrelationId = setCorrelationId;
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            _setCorrelationId.SetCorrelationId(context);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("SetCorrelationId");
        }
    }
}