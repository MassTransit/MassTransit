// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Util;


    public static class BusControlExtensions
    {
        /// <summary>
        /// Stop a bus, throwing an exception if the bus does not stop.
        /// It is a wrapper of the async method `StopAsync`
        /// </summary>
        /// <param name="busControl">The bus handle</param>
        public static void Stop(this IBusControl busControl)
        {
            TaskUtil.Await(() => busControl.StopAsync());
        }

        /// <summary>
        /// Starts a bus, throwing an exception if the bus does not start
        /// It is a wrapper of the async method `StartAsync`
        /// </summary>
        /// <param name="busControl">The bus handle</param>
        public static void Start(this IBusControl busControl)
        {
            TaskUtil.Await(() => busControl.StartAsync());
        }

        /// <summary>
        /// Stop a bus, throwing an exception if the bus does not stop in the specified timeout
        /// </summary>
        /// <param name="bus">The bus handle</param>
        /// <param name="stopTimeout">The wait time before throwing an exception</param>
        public static void Stop(this IBusControl bus, TimeSpan stopTimeout)
        {
            using (var cancellationTokenSource = new CancellationTokenSource(stopTimeout))
            {
                var cancellationToken = cancellationTokenSource.Token;

                TaskUtil.Await(() => bus.StopAsync(cancellationToken), cancellationToken);
            }
        }

        /// <summary>
        /// Stop a bus, throwing an exception if the bus does not stop in the specified timeout
        /// </summary>
        /// <param name="bus">The bus handle</param>
        /// <param name="stopTimeout">The wait time before throwing an exception</param>
        public static async Task StopAsync(this IBusControl bus, TimeSpan stopTimeout)
        {
            using (var cancellationTokenSource = new CancellationTokenSource(stopTimeout))
            {
                await bus.StopAsync(cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This can be used to start and stop the bus when configured in a deploy topology only scenario. No messages should be consumed by it.
        /// 
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task DeployAsync(IBusControl bus, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));

            await bus.StartAsync(cancellationToken).ConfigureAwait(false);

            await bus.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}