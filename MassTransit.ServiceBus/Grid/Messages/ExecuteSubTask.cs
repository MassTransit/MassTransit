// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Grid.Messages
{
    using System;

    [Serializable]
    public class ExecuteSubTask<TSubTask>
        where TSubTask : class
    {
        private readonly Uri _address;
        private readonly long _subTaskId;
        private readonly TSubTask _task;
        private readonly Guid _taskId;

        public ExecuteSubTask(Uri address, Guid taskId, long subTaskId, TSubTask task)
        {
            _address = address;
            _taskId = taskId;
            _subTaskId = subTaskId;
            _task = task;
        }

        public Uri Address
        {
            get { return _address; }
        }

        public Guid TaskId
        {
            get { return _taskId; }
        }

        public long SubTaskId
        {
            get { return _subTaskId; }
        }

        public TSubTask Task
        {
            get { return _task; }
        }
    }
}