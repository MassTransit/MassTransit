// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ApplicationInsights.Configuration
{
    using System;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using PublishPipeSpecifications;


    public class TelemetryPublishPipeSpecificationObserver :
        IPublishPipeSpecificationObserver
    {
        readonly TelemetryClient _telemetryClient;
        readonly Action<IOperationHolder<DependencyTelemetry>, PublishContext> _configureOperation;
        readonly string _telemetryHeaderRootKey;
        readonly string _telemetryHeaderParentKey;

        public TelemetryPublishPipeSpecificationObserver(TelemetryClient telemetryClient, string telemetryHeaderRootKey, string telemetryHeaderParentKey,
            Action<IOperationHolder<DependencyTelemetry>, PublishContext> configureOperation)
        {
            _telemetryClient = telemetryClient;
            _configureOperation = configureOperation;
            _telemetryHeaderRootKey = telemetryHeaderRootKey;
            _telemetryHeaderParentKey = telemetryHeaderParentKey;
        }

        void IPublishPipeSpecificationObserver.MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
        {
            var applicationInsightsSendSpecification =
                new ApplicationInsightsPublishSpecification<T>(_telemetryClient, _telemetryHeaderRootKey, _telemetryHeaderParentKey, _configureOperation);

            specification.AddPipeSpecification(applicationInsightsSendSpecification);
        }
    }
}