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
namespace MassTransit.Transformation.PropertyTransforms
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals.Reflection;


    public class ReplacePropertyTransform<TProperty, TInput> :
        IPropertyTransform<TInput>
        where TInput : class
    {
        readonly ReadWriteProperty<TInput, TProperty> _property;
        readonly TProperty _value;

        public ReplacePropertyTransform(PropertyInfo property, TProperty value)
        {
            _property = new ReadWriteProperty<TInput, TProperty>(property);
            _value = value;
        }

        public async Task Apply(TransformContext<TInput> context)
        {
            if (context.HasInput)
                _property.Set(context.Input, _value);
        }
    }
}