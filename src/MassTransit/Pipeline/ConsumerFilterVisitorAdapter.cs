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


    public class ConsumerFilterVisitorAdapter<TConsumer, TMessage> :
        IConsumerFilterVisitorAdapter
        where TConsumer : class, IConsumer<TMessage>
        where TMessage : class
    {
        public bool Visit<T>(IConsumerFilterVisitor visitor, IFilter<T> filter, FilterVisitorCallback callback)
            where T : class, PipeContext
        {
            var consumerMessageFilter = filter as IFilter<ConsumerConsumeContext<TConsumer, TMessage>>;
            if (consumerMessageFilter != null)
                return visitor.Visit(consumerMessageFilter, callback);

            var consumerFilter = filter as IFilter<ConsumerConsumeContext<TConsumer>>;
            if (consumerFilter != null)
                return visitor.Visit(consumerFilter, callback);

            throw new ArgumentException("The filter specified is not a consumer filter: " + filter.GetType().GetTypeName());
        }
    }
}