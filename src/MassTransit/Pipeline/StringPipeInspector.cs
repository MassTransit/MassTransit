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
namespace MassTransit.Pipeline
{
    using System.Text;
    using Filters;
    using Util;


    public class StringPipeInspector :
        PipeInspectorBase
    {
        readonly StringBuilder _builder = new StringBuilder();

        public override string ToString()
        {
            return _builder.ToString();
        }

        protected override bool Unknown<T>(IFilter<T> filter, FilterInspectorCallback callback)
        {
            _builder.AppendFormat("Unknown {0}", filter.GetType()).AppendLine();

            return base.Unknown(filter, callback);
        }

        protected override bool Inspect(MessageTypeConsumeFilter filter, FilterInspectorCallback callback)
        {
            _builder.AppendFormat("MessageTypeConsumerFilter").AppendLine();

            return base.Inspect(filter, callback);
        }

        protected override bool Inspect<T>(TeeConsumeFilter<T> filter, FilterInspectorCallback callback)
        {
            _builder.AppendFormat("{0}", TypeMetadataCache<TeeConsumeFilter<T>>.ShortName).AppendLine();

            return base.Inspect(filter, callback);
        }

        protected override bool Inspect<T>(HandlerMessageFilter<T> filter, FilterInspectorCallback callback)
        {
            _builder.AppendFormat("{0}", TypeMetadataCache<HandlerMessageFilter<T>>.ShortName).AppendLine(); ;

            return base.Inspect(filter, callback);
        }

        protected override bool Inspect<TConsumer, T>(ConsumerMessageFilter<TConsumer, T> filter, FilterInspectorCallback callback)
        {
            _builder.AppendFormat("{0}", TypeMetadataCache<ConsumerMessageFilter<TConsumer, T>>.ShortName).AppendLine(); ;

            return base.Inspect(filter, callback);
        }
    }
}