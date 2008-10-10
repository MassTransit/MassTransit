// Copyright 2007-2008 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//   http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Services.HealthMonitoring
{
    using System;

    public class HealthInformation
    {
        private readonly Uri _uri;
        private readonly int _secondsBetweenBeats;
        private readonly DateTime? _firstDetectedAt;
        private DateTime? _lastDetectedAt;
        private DateTime? _lastFaultDetectedAt;

        public HealthInformation(Uri uri, int secondsBetweenBeats)
        {
            _uri = uri;
            _secondsBetweenBeats = secondsBetweenBeats;
            _firstDetectedAt = DateTime.Now;
            _lastFaultDetectedAt = FirstDetectedAt;
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public int SecondsBetweenBeats
        {
            get { return _secondsBetweenBeats; }
        }

        public DateTime? FirstDetectedAt
        {
            get { return _firstDetectedAt; }
        }

        public DateTime? LastDetectedAt
        {
            get { return _lastDetectedAt; }
            set { _lastDetectedAt = value; }
        }

        public DateTime? LastFaultDetectedAt
        {
            get { return _lastFaultDetectedAt; }
            set { _lastFaultDetectedAt = value; }
        }
    }
}