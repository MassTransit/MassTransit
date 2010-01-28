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

    public class Message :
        NotifyPropertyChangedBase,
        IKeyedObject<string>
    {
        private Guid _clientId;
        private string _correlationId;
        private long _sequenceNumber;
        private Guid _subscriptionId;
        private Uri _endpointUri;

        private Message()
        {
        }

        public Message(string messageName) :
            this()
        {
            MessageName = messageName;
        }

        public string MessageName { get; set; }

        public string PrettyMessageName
        {
            get
            {
                string description = TypeNameHelper.ConverTypeStringToPrettyName(MessageName);

                if (!string.IsNullOrEmpty(CorrelationId))
                    description += " (" + CorrelationId + ")";
                return description;
            }
        }

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

        public Uri EndpointUri
        {
            get { return _endpointUri; }
            set
            {
                if (_endpointUri != value)
                {
                    _endpointUri = value;
                    OnPropertyChanged("EndpointUri");
                }
            }
        }

        public string Key
        {
            get { return MessageName; }
        }
    }
}