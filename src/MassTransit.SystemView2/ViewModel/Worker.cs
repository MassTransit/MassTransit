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
    using System.Collections.Generic;
    using System.Linq;

    public class Worker :
        NotifyPropertyChangedBase,
        IKeyedObject<string>
    {
        public string MessageType { get; set; }
        public string PrettyMessageType
        {
            get
            {
                var parts = MessageType.Split(',');
                var d = parts.Length > 0 ? parts[0] : MessageType;
                var dd = d.Split('.');

                string description = dd[dd.Length - 1];

                var gs = MessageType.Split('`');
                if (gs.Length > 1)
                {
                    var generics = new Queue<string>(gs.Reverse().Skip(1).Reverse());

                    while (generics.Count > 0)
                    {
                        var g = generics.Dequeue();
                        var gg = g.Split('.');
                        var ggg = gg.Length > 0 ? gg[gg.Length - 1] : g;

                        description = string.Format("{0}<{1}>", ggg, description);
                    }
                }
                return description;
            }
        }

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

        public Uri ControlUri { get; set; }
        public Uri DataUri { get; set; }
    }
}