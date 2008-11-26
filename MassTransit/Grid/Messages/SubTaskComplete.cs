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
    public class SubTaskComplete<TOutput>
        where TOutput : class
    {
        private int _activeTaskCount;
        private string _address;
        private TOutput _output;
        private int _subTaskId;
        private Guid _taskId;
        private int _taskLimit;

        protected SubTaskComplete()
        {
        }

        public SubTaskComplete(string address, int taskLimit, int activeTaskCount, Guid taskId, int subTaskId, TOutput output)
        {
            _address = address;
            _activeTaskCount = activeTaskCount;
            _taskLimit = taskLimit;
            _taskId = taskId;
            _subTaskId = subTaskId;
            _output = output;
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public int TaskLimit
        {
            get { return _taskLimit; }
            set { _taskLimit = value; }
        }

        public int ActiveTaskCount
        {
            get { return _activeTaskCount; }
            set { _activeTaskCount = value; }
        }

        public TOutput Output
        {
            get { return _output; }
            set { _output = value; }
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
    }
}