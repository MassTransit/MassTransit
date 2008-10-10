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
namespace MassTransit.ServiceBus.Services.HealthMonitoring.Messages
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class HealthStatusResponse :
        CorrelatedBy<Guid>
    {
        private readonly IList<HealthInformation> _healthInformation;
        private readonly Guid _correlationId;

        public HealthStatusResponse(IList<HealthInformation> healthInformation, Guid correlationId)
        {
            _healthInformation = healthInformation;
            _correlationId = correlationId;
        }

        public IList<HealthInformation> HealthInformation
        {
            get { return _healthInformation; }
        }

        #region Implementation of CorrelatedBy<Guid>

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        #endregion
    }
}