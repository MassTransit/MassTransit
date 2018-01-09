// Copyright 2012-2018 Chris Patterson
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
namespace GreenPipes.Agents
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public static class AsyncPipeContextHandleExtensions
    {
        /// <summary>
        /// Notify that the context creation task has completed and should be updated. If the Task has not yet completed,
        /// a continuation is added to it so that it is handled when completed.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="task"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task Notify<T>(this IAsyncPipeContextHandle<T> handle, Task<T> task)
            where T : class, PipeContext
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.IsCanceled)
            {
                handle.CreateCanceled();
            }
            else if (task.IsFaulted)
            {
                handle.CreateFaulted(task.Exception);
            }
            else if (task.IsCompleted)
            {
                handle.Created(task.Result);
            }
            else
            {
                Task NotifyAsync(Task<T> completedTask)
                {
                    if (task.IsCanceled)
                        handle.CreateCanceled();
                    else if (task.IsFaulted)
                        handle.CreateFaulted(task.Exception);
                    else
                        handle.Created(task.Result);

                    return TaskUtil.Completed;
                }

                return task.ContinueWith(NotifyAsync, TaskScheduler.Default);
            }

            return TaskUtil.Completed;
        }
    }
}