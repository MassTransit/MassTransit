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
    /// <summary>
    /// A property transform that is not dependent upon the input type
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IPropertyTransform<in TResult>
    {
        void Apply(TResult result, TransformContext context);
    }


    /// <summary>
    /// A property transform that uses the input type
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IPropertyTransform<in TResult, in TInput>
    {
        void Apply(TResult result, TransformContext<TInput> context);
    }
}