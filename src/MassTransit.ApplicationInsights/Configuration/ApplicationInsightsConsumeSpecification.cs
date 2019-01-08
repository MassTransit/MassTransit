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
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Pipeline;


    public class ApplicationInsightsConsumeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly Action<IOperationHolder<RequestTelemetry>, ConsumeContext> _configureOperation;
        readonly TelemetryClient _telemetryClient;
        readonly string _telemetryHeaderRootKey;
        readonly string _telemetryHeaderParentKey;

        public ApplicationInsightsConsumeSpecification(TelemetryClient telemetryClient, string telemetryHeaderRootKey, string telemetryHeaderParentKey,
            Action<IOperationHolder<RequestTelemetry>, ConsumeContext> configureOperation)
        {
            _telemetryClient = telemetryClient;
            _configureOperation = configureOperation;
            _telemetryHeaderRootKey = telemetryHeaderRootKey;
            _telemetryHeaderParentKey = telemetryHeaderParentKey;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(
                new ApplicationInsightsConsumeFilter<T>(_telemetryClient, _telemetryHeaderRootKey, _telemetryHeaderParentKey, _configureOperation));
        }
    }
}