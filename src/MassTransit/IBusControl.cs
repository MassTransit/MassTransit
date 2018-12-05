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
    using System.Threading;
    using System.Threading.Tasks;


    public interface IBusControl :
        IBus
    {
        /// <summary>
        /// Starts the bus (assuming the battery isn't dead). Once the bus has been started, it cannot be started again, even after is has been stopped.
        /// </summary>
        /// <returns>The BusHandle for the started bus. This is no longer needed, as calling Stop on the IBusControl will stop the bus equally well.</returns>
        Task<BusHandle> StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the bus if it has been started. If the bus hasn't been started, the method returns without any warning.
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}