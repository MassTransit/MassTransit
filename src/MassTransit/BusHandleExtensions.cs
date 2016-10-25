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
namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class BusHandleExtensions
    {
        /// <summary>
        /// Stop a bus, throwing an exception if the bus does not stop in the specified timeout
        /// </summary>
        /// <param name="handle">The bus handle</param>
        /// <param name="stopTimeout">The wait time before throwing an exception</param>
        public static void Stop(this BusHandle handle, TimeSpan stopTimeout)
        {
            using (var cancellationTokenSource = new CancellationTokenSource(stopTimeout))
            {
                handle.StopAsync(cancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// Stop a bus, throwing an exception if the bus does not stop in the specified timeout
        /// </summary>
        /// <param name="handle">The bus handle</param>
        /// <param name="stopTimeout">The wait time before throwing an exception</param>
        public static async Task StopAsync(this BusHandle handle, TimeSpan stopTimeout)
        {
            using (var cancellationTokenSource = new CancellationTokenSource(stopTimeout))
            {
                await handle.StopAsync(cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }
    }
}