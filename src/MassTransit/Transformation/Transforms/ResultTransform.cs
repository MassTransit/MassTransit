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
namespace MassTransit.Transformation.Transforms
{
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// Transforms the message by creating a new message and copying/setting/ignoring the original properties
    /// </summary>
    /// <typeparam name="TResult">The message type</typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class ResultTransform<TResult, TInput> :
        ITransform<TResult, TInput>
    {
        readonly IPropertyTransform<TResult, TInput>[] _propertyTransforms;
        readonly TransformResultFactory<TResult> _resultFactory;

        public ResultTransform(TransformResultFactory<TResult> resultFactory, IEnumerable<IPropertyTransform<TResult, TInput>> propertyTransforms)
        {
            _resultFactory = resultFactory;
            _propertyTransforms = propertyTransforms.ToArray();
        }

        public TransformResult<TResult> ApplyTo(TransformContext<TInput> context)
        {
            TResult result = _resultFactory();

            foreach (var propertyTransform in _propertyTransforms)
                propertyTransform.Apply(result, context);

            return context.Return(result);
        }
    }
}