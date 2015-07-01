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
    public static class TransformContextExtensions
    {
        /// <summary>
        /// Returns the original message as a transform result
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="isNewValue"></param>
        /// <returns></returns>
        public static TransformResult<TResult> Return<TResult>(this TransformContext context, TResult value, bool isNewValue = true)
        {
            return new Result<TResult>(value, isNewValue);
        }


        class Result<TResult> :
            TransformResult<TResult>
        {
            readonly bool _isNewValue;
            readonly TResult _value;

            public Result(TResult value, bool isNewValue)
            {
                _value = value;
                _isNewValue = isNewValue;
            }

            public TResult Value
            {
                get { return _value; }
            }

            public bool IsNewValue
            {
                get { return _isNewValue; }
            }
        }
    }
}