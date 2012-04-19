// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.Messages
{
    using System;
    using Magnum;

    /// <summary>
    /// Published by workers that can process a message of type T
    /// </summary>
    /// <typeparam name="T">The type of message that can be processed.</typeparam>
    public class WorkerAvailable<T> :
        Workload<T>,
        IWorkerAvailable
    {
        public WorkerAvailable(Uri controlUri, Uri dataUri, int inProgress, int inProgressLimit, int pending,
            int pendingLimit)
        {
            ControlUri = controlUri;
            DataUri = dataUri;
            InProgress = inProgress;
            InProgressLimit = inProgressLimit;
            Pending = pending;
            PendingLimit = pendingLimit;
            Updated = SystemUtil.UtcNow;
        }

        protected WorkerAvailable()
        {
        }

        public Uri ControlUri { get; set; }

        public Uri DataUri { get; set; }
        public DateTime Updated { get; set; }

        public string WorkerItemType
        {
            get { return typeof(T).FullName; }
        }

        public int Pending { get; set; }

        public int PendingLimit { get; set; }

        public int InProgress { get; set; }

        public int InProgressLimit { get; set; }
    }
}