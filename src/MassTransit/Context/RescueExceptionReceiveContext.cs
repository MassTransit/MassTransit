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
namespace MassTransit.Context
{
    using System;
    using Events;


    public class RescueExceptionReceiveContext :
        ReceiveContextProxy,
        ExceptionReceiveContext
    {
        readonly Exception _exception;
        readonly DateTime _exceptionTimestamp;
        ExceptionInfo _exceptionInfo;

        public RescueExceptionReceiveContext(ReceiveContext context, Exception exception)
            : base(context)
        {
            _exception = exception;

            _exceptionTimestamp = DateTime.UtcNow;
        }

        Exception ExceptionReceiveContext.Exception => _exception;
        DateTime ExceptionReceiveContext.ExceptionTimestamp => _exceptionTimestamp;

        ExceptionInfo ExceptionReceiveContext.ExceptionInfo
        {
            get
            {
                if (_exceptionInfo != null)
                    return _exceptionInfo;

                _exceptionInfo = new FaultExceptionInfo(_exception);

                return _exceptionInfo;
            }
        }
    }
}
