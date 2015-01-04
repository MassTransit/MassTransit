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
namespace MassTransit.Pipeline
{
    using System;
    using Internals.Extensions;


    public class ConsumeMessageFilterVisitorAdapter<TMessage> :
        IConsumeMessageFilterVisitorAdapter
        where TMessage : class
    {
        public bool Visit<T>(IConsumeMessageFilterVisitor visitor, IFilter<T> filter, FilterVisitorCallback callback)
            where T : class, PipeContext
        {
            var consumerMessageFilter = filter as IFilter<ConsumeContext<TMessage>>;
            if (consumerMessageFilter != null)
                return visitor.Visit(consumerMessageFilter, callback);

            throw new ArgumentException("The filter specified is not a consume message filter: " + filter.GetType().GetTypeName());
        }
    }
}