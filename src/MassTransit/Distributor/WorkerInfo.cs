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
namespace MassTransit.Distributor
{
    using System;
    using Magnum.Extensions;

    public class WorkerInfo :
        IWorker
    {
        public WorkerInfo(Uri controlUri, Uri dataUri)
        {
            ControlUri = controlUri;
            DataUri = dataUri;

            LastUpdate = DateTime.Now - 1.Days();
        }

        public int TotalPending { get; private set; }
        public int TotalPendingLimit { get; private set; }

        public int TotalInProgress { get; private set; }
        public int TotalInProgressLimit { get; private set; }

        public DateTime LastUpdate { get; private set; }
        public Uri ControlUri { get; private set; }
        public Uri DataUri { get; private set; }
    }


    public class WorkerInfo<TMessage> :
        IWorker<TMessage>
        where TMessage : class
    {
        readonly object _locker = new object();
        readonly IWorker _worker;

        public WorkerInfo(IWorker worker)
        {
            _worker = worker;
        }

        public DateTime LastUpdate { get; private set; }

        public void Assigned()
        {
            lock (_locker)
            {
                InProgress++;
                DateTime now = DateTime.UtcNow;
                if (now > LastUpdate)
                    LastUpdate = now;
            }
        }

        public Uri ControlUri
        {
            get { return _worker.ControlUri; }
        }

        public Uri DataUri
        {
            get { return _worker.DataUri; }
        }

        public int Pending { get; private set; }
        public int PendingLimit { get; private set; }
        public int InProgress { get; private set; }
        public int InProgressLimit { get; private set; }

        public void Update(int inProgress, int inProgressLimit, int pending, int pendingLimit, DateTime updated)
        {
            lock (_locker)
            {
                if (updated < LastUpdate)
                    return;

                InProgress = inProgress;
                InProgressLimit = inProgressLimit;

                Pending = pending;
                PendingLimit = pendingLimit;
            }
        }
    }
}