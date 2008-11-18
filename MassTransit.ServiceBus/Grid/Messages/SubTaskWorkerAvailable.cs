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
    using ServiceBus;

    [Serializable]
    [ExpiresIn("00:01:00")]
    public class SubTaskWorkerAvailable<TInput>
    {
        private int _activeTaskCount;
        private string _address;
        private int _taskLimit;

        protected SubTaskWorkerAvailable()
        {
        }

        public SubTaskWorkerAvailable(string address, int taskLimit, int activeTaskCount)
        {
            _address = address;
            _activeTaskCount = activeTaskCount;
            _taskLimit = taskLimit;
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
    }
}