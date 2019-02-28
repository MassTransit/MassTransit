// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ApplicationInsights.Tests
{
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    internal class CopyPropertiesTelemetryInitializer : ITelemetryInitializer
    {
        private IDictionary<string, string> Properties { get; }

        public CopyPropertiesTelemetryInitializer(ISupportProperties source)
        {
            Properties = source.Properties;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var itemWithProperties = (ISupportProperties)telemetry;
            foreach (var property in Properties)
            {
                itemWithProperties.Properties.Add(property);
            }
        }
    }
}
