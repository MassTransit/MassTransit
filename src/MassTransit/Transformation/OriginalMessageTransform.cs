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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    /// <summary>
    /// Transforms the original message, in-place, without generating a new message result.
    /// This is best used for lazy, one-time replacement of message properties that have
    /// an immutable value.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class OriginalMessageTransform<TMessage> :
        IMessageTransform<TMessage>
        where TMessage : class
    {
        readonly IPropertyTransform<TMessage>[] _propertyTransforms;

        public OriginalMessageTransform(IEnumerable<IPropertyTransform<TMessage>> propertyTransforms)
        {
            _propertyTransforms = propertyTransforms.ToArray();
        }

        public Task<TransformResult<TMessage>> ApplyTo(TransformContext<TMessage> context)
        {
            foreach (var propertyTransform in _propertyTransforms)
                propertyTransform.Apply(context);

            return context.ReturnOriginal();
        }
    }
}