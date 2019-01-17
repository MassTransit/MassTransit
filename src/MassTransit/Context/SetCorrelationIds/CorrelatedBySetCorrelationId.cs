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
namespace MassTransit.Context.SetCorrelationIds
{
    using System;
    using Internals.Reflection;


    public class CorrelatedBySetCorrelationId<T> :
        ISetCorrelationId<T>
        where T : class, CorrelatedBy<Guid>
    {
        readonly IReadProperty<CorrelatedBy<Guid>, Guid> _property;

        public CorrelatedBySetCorrelationId()
        {
            _property = ReadPropertyCache<CorrelatedBy<Guid>>.GetProperty<Guid>(nameof(CorrelatedBy<Guid>.CorrelationId));
        }

        public void SetCorrelationId(SendContext<T> context)
        {
            var message = (CorrelatedBy<Guid>)context.Message;

            var correlationId = _property.Get(message);
            if (correlationId != Guid.Empty)
                context.CorrelationId = correlationId;
        }
    }
}