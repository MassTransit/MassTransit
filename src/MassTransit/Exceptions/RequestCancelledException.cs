// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Util;

    [Serializable]
    public class RequestCancelledException :
        MassTransitException
    {
        public RequestCancelledException()
        {
        }

        public RequestCancelledException(string requestId)
            : base(FormatMessage(requestId))
        {
        }

        public RequestCancelledException(string requestId, Exception innerException)
            : base(FormatMessage(requestId), innerException)
        {
        }

        protected RequestCancelledException([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static string FormatMessage(string requestId)
        {
            return string.Format("The request was cancelled, RequestId: {0}", requestId);
        }
    }
}