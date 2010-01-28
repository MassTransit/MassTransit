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
namespace MassTransit.SystemView.Core.ViewModel
{
    using System;

    public class Worker :
        NotifyPropertyChangedBase,
        IKeyedObject<string>
    {
        private DateTime _updated;
        private int _pending;
        private int _pendingLimit;
        private int _inProgress;
        private int _inProgressLimit;

        private Worker()
        {
        }

        public Worker(string messageType) :
            this()
        {
            MessageType = messageType;
        }

        public string MessageType { get; set; }
        public string PrettyMessageType
        {
            get
            {
                return TypeNameHelper.ConverTypeStringToPrettyName(MessageType);
            }
        }

        public string Key
        {
            get { return MessageType; }
        }

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

        public Uri ControlUri { get; set; }
        public Uri DataUri { get; set; }
    }
}