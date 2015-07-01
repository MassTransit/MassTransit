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
namespace MassTransit.Transformation.Contexts
{
    using System;
    using System.Threading;


    public class TransformSourceContext<TProperty, TInput> :
        SourceContext<TProperty, TInput>
    {
        readonly TransformContext<TInput> _context;
        readonly bool _hasValue;
        readonly TProperty _value;

        public TransformSourceContext(TransformContext<TInput> context)
        {
            _context = context;
        }

        public TransformSourceContext(TransformContext<TInput> context, TProperty value)
        {
            _context = context;
            _value = value;
            _hasValue = true;
        }

        CancellationToken TransformContext.CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        bool TransformContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool TransformContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        TPayload TransformContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public TInput Input
        {
            get { return _context.Input; }
        }

        public bool HasInput
        {
            get { return _context.HasInput; }
        }

        public bool HasValue
        {
            get { return _hasValue; }
        }

        public TProperty Value
        {
            get { return _value; }
        }
    }
}