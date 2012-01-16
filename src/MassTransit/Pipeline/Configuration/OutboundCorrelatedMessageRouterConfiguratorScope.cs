// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Configuration
{
    using Inspectors;
	using Sinks;
	using Util;

	public class OutboundCorrelatedMessageRouterConfiguratorScope<TMessage, TKey> :
		PipelineInspectorBase<OutboundCorrelatedMessageRouterConfiguratorScope<TMessage, TKey>>
		where TMessage : class, CorrelatedBy<TKey>
	{
		public CorrelatedMessageRouter<IBusPublishContext<TMessage>, TMessage, TKey> Router { get; private set; }

		[UsedImplicitly]
		public bool Inspect<T, TM, TK>(CorrelatedMessageRouter<T, TM, TK> router)
			where T : class, IMessageContext<TM>
			where TM : class, CorrelatedBy<TK>
		{
			if (typeof (T) == typeof (IBusPublishContext<TMessage>) && typeof (TM) == typeof (TMessage) &&
			    typeof (TK) == typeof (TKey))
			{
				Router = router.TranslateTo<CorrelatedMessageRouter<IBusPublishContext<TMessage>, TMessage, TKey>>();

				return false;
			}

			return true;
		}
	}
}