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
namespace MassTransit.Saga.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    /// <summary>
    /// Extracts the CorrelationId from the message where there is a one-to-one correlation
    /// identifier in the message (such as CorrelationId) and sets it in the header for use
    /// by the saga repository.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class CorrelationIdMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly Func<ConsumeContext<TMessage>, Guid> _getCorrelationId;

        public CorrelationIdMessageFilter(Func<ConsumeContext<TMessage>, Guid> getCorrelationId)
        {
            if (getCorrelationId == null)
                throw new ArgumentNullException(nameof(getCorrelationId));

            _getCorrelationId = getCorrelationId;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("correlationId");
        }

        public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Guid correlationId = _getCorrelationId(context);

            var proxy = new CorrelationIdConsumeContextProxy<TMessage>(context, correlationId);

            return next.Send(proxy);
        }
    }
}