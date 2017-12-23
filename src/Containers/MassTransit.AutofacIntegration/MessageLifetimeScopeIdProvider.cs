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
namespace MassTransit.AutofacIntegration
{
    /// <summary>
    /// Gets the LifetimeScope Id using the message
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class MessageLifetimeScopeIdProvider<TMessage, TId> :
        ILifetimeScopeIdProvider<TId>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ILifetimeScopeIdAccessor<TMessage, TId> _accessor;

        public MessageLifetimeScopeIdProvider(ConsumeContext<TMessage> consumeContext, ILifetimeScopeIdAccessor<TMessage, TId> accessor)
        {
            _consumeContext = consumeContext;
            _accessor = accessor;
        }

        public bool TryGetScopeId(out TId id)
        {
            return _accessor.TryGetScopeId(_consumeContext.Message, out id);
        }
    }
}