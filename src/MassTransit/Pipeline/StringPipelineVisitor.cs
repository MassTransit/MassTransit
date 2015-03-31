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
    using System.Linq;
    using System.Text;
    using Filters;
    using Internals.Extensions;
    using Util;


    public class StringPipelineVisitor :
        PipelineVisitor
    {
        readonly StringBuilder _builder = new StringBuilder();

        public override string ToString()
        {
            return _builder.ToString();
        }

        protected override bool VisitUnknownFilter<T>(IFilter<T> filter, FilterVisitorCallback callback)
        {
            _builder.AppendFormat("VisitUnknownFilter {0}", filter.GetType().GetTypeName()).AppendLine();

            return base.VisitUnknownFilter(filter, callback);
        }

        protected override bool VisitMessageTypeConsumeFilter(MessageTypeConsumeFilter filter, FilterVisitorCallback callback)
        {
            _builder.AppendFormat("MessageTypeConsumerFilter").AppendLine();

            return base.VisitMessageTypeConsumeFilter(filter, callback);
        }

        protected override bool VisitTeeConsumeFilter<T>(TeeConsumeFilter<T> filter, FilterVisitorCallback callback)
        {
            _builder.AppendFormat("{0}", TypeMetadataCache<TeeConsumeFilter<T>>.ShortName).AppendLine();

            return base.VisitTeeConsumeFilter(filter, callback);
        }

        protected override bool VisitHandlerMessageFilter<T>(HandlerMessageFilter<T> filter, FilterVisitorCallback callback)
        {
            _builder.AppendFormat("Handler({0})", TypeMetadataCache<T>.ShortName).AppendLine();
            ;

            return base.VisitHandlerMessageFilter(filter, callback);
        }

        protected override bool VisitRetryConsumeFilter(RetryFilter filter, FilterVisitorCallback callback)
        {
            _builder.AppendFormat("Retry({0}) - {1}", TypeMetadataCache<ConsumeContext>.ShortName, filter.RetryPolicy).AppendLine();

            return base.VisitRetryConsumeFilter(filter, callback);
        }

        protected override bool VisitRetryConsumeFilter<T>(RetryFilter<T> filter, FilterVisitorCallback callback)
        {
            _builder.AppendFormat("Retry({0}) - {1}", TypeMetadataCache<ConsumeContext<T>>.ShortName, filter.RetryPolicy).AppendLine();

            return base.VisitRetryConsumeFilter(filter, callback);
        }

        protected override bool VisitMethodConsumerMessageFilter<TConsumer, T>(MethodConsumerMessageFilter<TConsumer, T> filter, FilterVisitorCallback callback)
        {
            _builder.AppendFormat("Task {0}.Consume(ConsumeContext<{1}> context)", TypeMetadataCache<TConsumer>.ShortName, TypeMetadataCache<T>.ShortName)
                .AppendLine();

            return base.VisitMethodConsumerMessageFilter(filter, callback);
        }

        protected override bool VisitConsumerSplitFilter<TConsumer, T>(ConsumerSplitFilter<TConsumer, T> filter, FilterVisitorCallback callback)
        {
            _builder.AppendFormat("{0}", TypeMetadataCache<ConsumerSplitFilter<TConsumer, T>>.ShortName).AppendLine();

            return base.VisitConsumerSplitFilter(filter, callback);
        }

        protected override bool VisitConsumerMessageFilter<T>(IFilter<ConsumeContext<T>> filter, FilterVisitorCallback callback)
        {
            Type consumerType = filter.GetType().GetGenericArguments().First();
            _builder.AppendFormat("Consumer {0} of {1}", consumerType.GetTypeName(), TypeMetadataCache<T>.ShortName)
                .AppendLine();

            return base.VisitConsumerMessageFilter(filter, callback);
        }
    }
}