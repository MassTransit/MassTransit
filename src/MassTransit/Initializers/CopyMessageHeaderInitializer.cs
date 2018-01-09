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


    /// <summary>
    /// Set a header to a constant value from the input
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="THeader">The header type</typeparam>
    public class CopyMessageHeaderInitializer<TMessage, TInput, THeader> :
        IMessageHeaderInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IWriteProperty<SendContext<TMessage>, THeader> _headerProperty;
        readonly IReadProperty<TInput, THeader> _inputProperty;

        public CopyMessageHeaderInitializer(string headerName, string inputPropertyName = null)
        {
            if (headerName == null)
                throw new ArgumentNullException(nameof(headerName));

            _inputProperty = ReadPropertyCache<TInput>.GetProperty<THeader>(inputPropertyName ?? headerName);
            _headerProperty = WritePropertyCache<SendContext<TMessage>>.GetProperty<THeader>(headerName);
        }

        public Task Apply(InitializeMessageContext<TMessage, TInput> context, SendContext<TMessage> sendContext)
        {
            var inputPropertyValue = _inputProperty.Get(context.Input);

            _headerProperty.Set(sendContext, inputPropertyValue);

            return TaskUtil.Completed;
        }
    }
}