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
namespace MassTransit.Util
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface ITaskParticipant
    {
        /// <summary>
        /// Completed when the participant is ready
        /// </summary>
        Task ParticipantReady { get; }

        /// <summary>
        /// An awaitable task that is completed when stop is requested
        /// </summary>
        Task<IStopEvent> StopRequested { get; }

        /// <summary>
        /// Cancelled when the participant is stopping
        /// </summary>
        CancellationToken StoppingToken { get; }

        /// <summary>
        /// Cancelled when the participant has stopped
        /// </summary>
        CancellationToken StoppedToken { get; }

        /// <summary>
        /// Completed when the participant is completed
        /// </summary>
        Task ParticipantCompleted { get; }

        /// <summary>
        /// Signal that the participant has completed use of the resource
        /// </summary>
        void SetComplete();

        /// <summary>
        /// Signal that the participant is actively using the resource
        /// </summary>
        void SetReady();

        /// <summary>
        /// Signal that the participant failed to become ready
        /// </summary>
        /// <param name="exception"></param>
        void SetNotReady(Exception exception);

        /// <summary>
        /// Stop the participant
        /// </summary>
        /// <param name="stopEvent"></param>
        void Stop(IStopEvent stopEvent);
    }
}