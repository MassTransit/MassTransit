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
namespace MassTransit.Policies
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class Repeat
    {
        /// <summary>
        /// Repeat until cancelled using the cancellationToken
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IRepeatPolicy UntilCancelled(CancellationToken cancellationToken)
        {
            return new UntilCancelledRepeatPolicy(cancellationToken);
        }

        public static async Task UntilCancelled(CancellationToken cancellationToken, Func<Task> callback)
        {
            await Task.Yield();

            IRepeatPolicy repeatPolicy = UntilCancelled(cancellationToken);
            using (IRepeatContext repeatContext = repeatPolicy.GetRepeatContext())
            {
                TimeSpan delay = TimeSpan.Zero;
                do
                {
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, repeatContext.CancellationToken);

                    await callback();
                }
                while (repeatContext.CanRepeat(out delay));
            }
        }
    }
}