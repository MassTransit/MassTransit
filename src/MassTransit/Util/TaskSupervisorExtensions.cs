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
namespace MassTransit.Util
{
    using System;
    using System.Threading.Tasks;
    using Logging;


    public static class TaskSupervisorExtensions
    {
        static readonly ILog _log = Logger.Get<TaskSupervisor>();

        public static ITaskScope CreateScope(this ITaskSupervisor supervisor, string tag, Func<Task> onStopMethod)
        {
            var scope = supervisor.CreateScope(tag);

            scope.StopRequested.ContinueWith(async stopTask =>
            {
                var stopEvent = await stopTask.ConfigureAwait(false);

                await scope.Stop(stopEvent.Reason).ConfigureAwait(false);

                try
                {
                    await onStopMethod().ConfigureAwait(false);

                    await scope.Completed.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _log.Error($"Failed to close scope {tag}", ex);
                }
                finally
                {
                    scope.SetComplete();
                }
            });

            return scope;
        }

        public static ITaskParticipant CreateParticipant(this ITaskSupervisor supervisor, string tag, Func<Task> onStopMethod)
        {
            var participant = supervisor.CreateParticipant(tag);

            participant.StopRequested.ContinueWith(async stopTask =>
            {
                try
                {
                    await onStopMethod().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _log.Error($"Failed to close scope {tag}", ex);
                }
                finally
                {
                    participant.SetComplete();
                }
            });

            return participant;
        }
    }
}