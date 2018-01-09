// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using Context;


    public static class RetryContextExtensions
    {
        /// <summary>
        /// If within a retry attempt, the return value is greater than zero and indicates the number of retry attempts
        /// that have occurred.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The retry attempt number, 0 = first time, >= 1 = retry</returns>
        public static int GetRetryAttempt(this ConsumeContext context)
        {
            return context.TryGetPayload(out ConsumeRetryContext retryContext) ? retryContext.RetryAttempt : 0;
        }
    }
}