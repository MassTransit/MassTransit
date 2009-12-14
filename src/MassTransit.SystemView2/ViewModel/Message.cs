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

    public class Message :
        NotifyPropertyChangedBase
    {
        public string MessageName { get; set; }

        public string PrettyMessageName
        {
            get
            {
                var parts = MessageName.Split(',');
                var d = parts.Length > 0 ? parts[0] : MessageName;
                var dd = d.Split('.');

                string description = dd[dd.Length - 1];

                var gs = MessageName.Split('`');
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

                if (!string.IsNullOrEmpty(CorrelationId))
                    description += " (" + CorrelationId + ")";
                return description;
            }
        }

            private Guid _clientId;
        public Guid ClientId
        {
            get { return _clientId; }
            set
            {
                if (_clientId != value)
                {
                    _clientId = value;
                    OnPropertyChanged("ClientId");
                }
            }
        }

        private string _correlationId;
        public string CorrelationId
        {
            get { return _correlationId; }
            set
            {
                if (_correlationId != value)
                {
                    _correlationId = value;
                    OnPropertyChanged("CorrelationId");
                }
            }
        }

        private long _sequenceNumber;
        public long SequenceNumber
        {
            get { return _sequenceNumber; }
            set
            {
                if (_sequenceNumber != value)
                {
                    _sequenceNumber = value;
                    OnPropertyChanged("SequenceNumber");
                }
            }
        }

        private Guid _subscriptionId;
        public Guid SubscriptionId
        {
            get { return _subscriptionId; }
            set
            {
                if (_subscriptionId != value)
                {
                    _subscriptionId = value;
                    OnPropertyChanged("SubscriptionId");
                }
            }
        }
    }
}