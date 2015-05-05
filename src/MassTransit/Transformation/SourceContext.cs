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
    public interface SourceContext :
        TransformContext
    {
    }


    public interface SourceContext<out TInput> :
        SourceContext
    {
        /// <summary>
        /// The input of the transform
        /// </summary>
        TInput Input { get; }

        /// <summary>
        /// True if the input is present
        /// </summary>
        bool HasInput { get; }
    }


    /// <summary>
    /// Provides the context for a property transform
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public interface SourceContext<out TProperty, out TInput> :
        SourceContext<TInput>
    {
        /// <summary>
        /// True if the value is present from the source
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// The value
        /// </summary>
        TProperty Value { get; }
    }
}