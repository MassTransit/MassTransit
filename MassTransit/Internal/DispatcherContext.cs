// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Internal
{
	using Subscriptions;

    public class DispatcherContext :
        IDispatcherContext
    {
        private readonly IObjectBuilder _builder;
        private readonly IServiceBus _bus;
    	private readonly ISubscriptionCache _cache;

    	public DispatcherContext(IObjectBuilder builder, IServiceBus bus, ISubscriptionCache cache)
        {
            _builder = builder;
    		_cache = cache;
    		_bus = bus;
        }

        public IObjectBuilder Builder
        {
            get { return _builder; }
        }

        public IServiceBus Bus
        {
            get { return _bus; }
        }

        public ISubscriptionCache SubscriptionCache
        {
            get { return _cache; }
        }
    }
}