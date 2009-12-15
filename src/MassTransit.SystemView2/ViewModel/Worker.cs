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
namespace MassTransit.SystemView.ViewModel
{
    using System;

    public class Worker :
        NotifyPropertyChangedBase,
        IKeyedObject<string>
    {
        public string MessageType { get; set; }

        public string Key
        {
            get { return MessageType; }
        }

        private int _pending;
        public int Pending
        {
            get { return _pending; }
            set
            {
                if (_pending != value)
                {
                    _pending = value;
                    OnPropertyChanged("Pending");
                }
            }
        }

        private int _pendingLimit;
        public int PendingLimit
        {
            get { return _pendingLimit; }
            set
            {
                if (_pendingLimit != value)
                {
                    _pendingLimit = value;
                    OnPropertyChanged("PendingLimit");
                }
            }
        }

        private int _inProgress;
        public int InProgress
        {
            get { return _inProgress; }
            set
            {
                if (_inProgress != value)
                {
                    _inProgress = value;
                    OnPropertyChanged("InProgress");
                }
            }
        }

        private int _inProgressLimit;
        public int InProgressLimit
        {
            get { return _inProgressLimit; }
            set
            {
                if (_inProgressLimit != value)
                {
                    _inProgressLimit = value;
                    OnPropertyChanged("InProgressLimit");
                }
            }
        }

        private DateTime _updated;
        public DateTime Updated
        {
            get { return _updated; }
            set
            {
                if (_updated != value)
                {
                    _updated = value;
                    OnPropertyChanged("Updated");
                }
            }
        }
    }
}