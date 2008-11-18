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
    public class ExecuteSubTask<TInput>
        where TInput : class
    {
        private string _address;
        private int _subTaskId;
        private TInput _task;
        private Guid _taskId;

        protected ExecuteSubTask()
        {
        }

        public ExecuteSubTask(string address, Guid taskId, int subTaskId, TInput task)
        {
            _address = address;
            _taskId = taskId;
            _subTaskId = subTaskId;
            _task = task;
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public Guid TaskId
        {
            get { return _taskId; }
            set { _taskId = value; }
        }

        public int SubTaskId
        {
            get { return _subTaskId; }
            set { _subTaskId = value; }
        }

        public TInput Task
        {
            get { return _task; }
            set { _task = value; }
        }
    }
}