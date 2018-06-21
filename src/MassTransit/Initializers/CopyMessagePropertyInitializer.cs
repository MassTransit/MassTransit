// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Initializers
{
    using System;
    using System.Threading.Tasks;
    using Internals.Reflection;
    using Util;


    public class CopyMessagePropertyInitializer<TMessage, TInput, TProperty> :
        IMessagePropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IReadProperty<TInput, TProperty> _inputProperty;
        readonly IWriteProperty<TMessage, TProperty> _messageProperty;

        public CopyMessagePropertyInitializer(string messagePropertyName, string inputPropertyName = null)
        {
            if (messagePropertyName == null)
                throw new ArgumentNullException(nameof(messagePropertyName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TProperty>(inputPropertyName ?? messagePropertyName);
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<TProperty>(messagePropertyName);
        }

        public Task Apply(InitializeMessageContext<TMessage, TInput> context)
        {
            _messageProperty.Set(context.Message, _inputProperty.Get(context.Input));

            return TaskUtil.Completed;
        }
    }


    public class ConvertMessagePropertyInitializer<TMessage, TInput, TProperty, TInputProperty> :
        IMessagePropertyInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IPropertyTypeConverter<TProperty, TInputProperty> _converter;
        readonly IReadProperty<TInput, TInputProperty> _inputProperty;
        readonly IWriteProperty<TMessage, TProperty> _messageProperty;

        public ConvertMessagePropertyInitializer(IPropertyTypeConverter<TProperty, TInputProperty> converter, string messagePropertyName,
            string inputPropertyName = null)
        {
            if (messagePropertyName == null)
                throw new ArgumentNullException(nameof(messagePropertyName));

            _converter = converter;

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<TInputProperty>(inputPropertyName ?? messagePropertyName);
            _messageProperty = WritePropertyCache<TMessage>.GetProperty<TProperty>(messagePropertyName);
        }

        public Task Apply(InitializeMessageContext<TMessage, TInput> context)
        {
            var inputProperty = _inputProperty.Get(context.Input);

            if (_converter.TryConvert(inputProperty, out var messageProperty))
            {
                _messageProperty.Set(context.Message, messageProperty);
                return TaskUtil.Completed;
            }

            throw new MassTransitException($"The input property {_inputProperty.Name} could not be converted to the message property {_messageProperty.Name}.");
        }
    }
}