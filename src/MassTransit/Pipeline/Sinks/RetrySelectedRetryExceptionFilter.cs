// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Sinks
{
    using System;


    public class RetrySelectedRetryExceptionFilter :
        IRetryExceptionFilter
    {
        readonly Type[] _exceptionTypes;

        public RetrySelectedRetryExceptionFilter(params Type[] exceptionTypes)
        {
            _exceptionTypes = exceptionTypes;
        }

        public bool CanRetry(Exception exception)
        {
            for (int i = 0; i < _exceptionTypes.Length; i++)
            {
                if (_exceptionTypes[i].IsInstanceOfType(exception))
                    return true;
            }

            return false;
        }
    }
}