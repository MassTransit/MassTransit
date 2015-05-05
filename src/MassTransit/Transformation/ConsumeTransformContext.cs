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
namespace MassTransit.Transformation
{
    using System;
    using System.Threading;
    using Context;


    /// <summary>
    /// Sits in front of the consume context and allows the inbound message to be 
    /// transformed.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumeTransformContext<TMessage> :
        TransformContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly PayloadCache _payloadCache;

        public ConsumeTransformContext(ConsumeContext<TMessage> context)
        {
            _context = context;
            _payloadCache = new PayloadCache();
        }

        CancellationToken TransformContext.CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        bool TransformContext.HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType) || _context.HasPayloadType(contextType);
        }

        bool TransformContext.TryGetPayload<TPayload>(out TPayload context)
        {
            if (_payloadCache.TryGetPayload(out context))
                return true;

            return _context.TryGetPayload(out context);
        }

        TPayload TransformContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            TPayload payload;
            if (_payloadCache.TryGetPayload(out payload))
                return payload;

            if (_context.TryGetPayload(out payload))
                return payload;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        TMessage TransformContext<TMessage>.Input
        {
            get { return _context.Message; }
        }

        bool TransformContext<TMessage>.HasInput
        {
            get { return true; }
        }

        TransformResult<TResult> TransformContext<TMessage>.Return<TResult>(TResult value, bool isNewValue)
        {
            return new Result<TResult>(value, isNewValue);
        }


        class Result<TResult> :
            TransformResult<TResult>
        {
            readonly bool _isNewValue;
            readonly TResult _value;

            public Result(TResult value, bool isNewValue)
            {
                _value = value;
                _isNewValue = isNewValue;
            }

            public TResult Value
            {
                get { return _value; }
            }

            public bool IsNewValue
            {
                get { return _isNewValue; }
            }
        }
    }
}