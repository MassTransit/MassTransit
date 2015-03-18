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
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Threading;
    using Context;


    class TestPipeContext :
        PipeContext
    {
        readonly PayloadCache _cache;

        public TestPipeContext()
        {
            _cache = new PayloadCache();
        }

        public CancellationToken CancellationToken
        {
            get { return default(CancellationToken); }
        }

        public bool HasPayloadType(Type contextType)
        {
            return _cache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context) where TPayload : class
        {
            return _cache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _cache.GetOrAddPayload(payloadFactory);
        }
    }
}