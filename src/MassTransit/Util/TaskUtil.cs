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
    using System.Threading.Tasks;


    static class TaskUtil
    {
        internal static Task Canceled
        {
            get { return Cached<bool>.CanceledTask; }
        }

        internal static Task Completed
        {
            get { return Cached.CompletedTask; }
        }


        static class Cached
        {
            public static readonly Task CompletedTask = Task.FromResult(true);
        }


        static class Cached<T>
        {
            public static readonly Task<T> CanceledTask = GetCanceledTask();

            static Task<T> GetCanceledTask()
            {
                var source = new TaskCompletionSource<T>();
                source.SetCanceled();
                return source.Task;
            }
        }
    }
}