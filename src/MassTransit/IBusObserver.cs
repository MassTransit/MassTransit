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
    using System.Threading.Tasks;


    /// <summary>
    /// Used to observe events produced by the bus
    /// </summary>
    public interface IBusObserver
    {
        /// <summary>
        /// Called after the bus has been created.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        Task PostCreate(IBus bus);

        /// <summary>
        /// Called when the bus fails to be created
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task CreateFaulted(IBus bus, Exception exception);

        /// <summary>
        /// Called when the bus is being started, before the actual Start commences.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        Task PreStart(IBus bus);

        /// <summary>
        /// Called once the bus has started and is running
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        Task PostStart(IBus bus);

        /// <summary>
        /// Called when the bus fails to start
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task StartFaulted(IBus bus, Exception exception);

        /// <summary>
        /// Called when the bus is being stopped, before the actual Stop commences.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        Task PreStop(IBus bus);

        /// <summary>
        /// Called when the bus has been stopped.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        Task PostStop(IBus bus);

        /// <summary>
        /// Called when the bus failed to Stop.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task StopFaulted(IBus bus, Exception exception);
    }
}