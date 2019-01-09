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
namespace MassTransit.MessageData
{
    using System.Reflection;
    using Internals.Reflection;
    using Transformation;


    public class LoadMessageDataPropertyProvider<TInput, TValue> :
        IPropertyProvider<MessageData<TValue>, TInput>
    {
        readonly IReadProperty<TInput, MessageData<TValue>> _property;
        readonly IMessageDataRepository _repository;

        public LoadMessageDataPropertyProvider(IMessageDataRepository repository, PropertyInfo property)
        {
            _repository = repository;
            _property = ReadPropertyCache<TInput>.GetProperty<MessageData<TValue>>(property);
        }

        public MessageData<TValue> GetProperty(TransformContext<TInput> context)
        {
            if (context.HasInput)
            {
                MessageData<TValue> value = _property.Get(context.Input);
                if (value?.Address != null)
                    return MessageDataFactory.Load<TValue>(_repository, value.Address, context);
            }

            return new EmptyMessageData<TValue>();
        }
    }
}