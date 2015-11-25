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
namespace MassTransit.Events
{
    using System;
    using Internals.Extensions;
    using Util;

    [Serializable]
    public class FaultExceptionInfo :
        ExceptionInfo
    {
        readonly Exception _exception;

        public FaultExceptionInfo(Exception exception)
        {
            _exception = exception;
        }

        public string ExceptionType => _exception.GetType().GetTypeName();

        public ExceptionInfo InnerException
        {
            get
            {
                if (_exception.InnerException != null)
                    return new FaultExceptionInfo(_exception.InnerException);
                return null;
            }
        }

        public string StackTrace => ExceptionUtil.GetStackTrace(_exception);

        public string Message => _exception.Message;

        public string Source => _exception.Source;
    }
}