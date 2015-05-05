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


    public class CopyMessageTransform<TResult, TInput> :
        ITransform<TResult, TInput>
        where TInput : TResult
    {
        readonly IPropertyTransform<TInput, TInput>[] _propertyTransforms;

        public CopyMessageTransform(IEnumerable<IPropertyTransform<TInput, TInput>> propertyTransforms)
        {
            _propertyTransforms = propertyTransforms.ToArray();
        }

        public TransformResult<TResult> ApplyTo(TransformContext<TInput> context)
        {
            foreach (var propertyTransform in _propertyTransforms)
                propertyTransform.Apply(context.Input, context);

            return context.Return<TResult>(context.Input, false);
        }
    }
}