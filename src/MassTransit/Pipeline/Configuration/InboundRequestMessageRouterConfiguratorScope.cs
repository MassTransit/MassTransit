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

    public class InboundRequestMessageRouterConfiguratorScope<TMessage> :
        PipelineInspectorBase<InboundRequestMessageRouterConfiguratorScope<TMessage>>
        where TMessage : class
    {
        public RequestMessageRouter<IConsumeContext<TMessage>, TMessage> Router { get; private set; }

        [UsedImplicitly]
        public bool Inspect<T, TM>(RequestMessageRouter<T, TM> router)
            where T : class, IMessageContext<TM>
            where TM : class
        {
            if (typeof (T) == typeof (IConsumeContext<TMessage>) && typeof (TM) == typeof (TMessage))
            {
                Router = router.TranslateTo<RequestMessageRouter<IConsumeContext<TMessage>, TMessage>>();

                return false;
            }

            return true;
        }
    }
}