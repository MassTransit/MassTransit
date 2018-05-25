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
namespace MassTransit.ApplicationInsights
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Microsoft.ApplicationInsights;


    public class ApplicationInsightsSpecification<T> : IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsSpecification(TelemetryClient telemetryClient)
            => _telemetryClient = telemetryClient;

        public IEnumerable<ValidationResult> Validate()
            => Enumerable.Empty<ValidationResult>();

        public void Apply(IPipeBuilder<T> builder)
            => builder.AddFilter(new ApplicationInsightsFilter<T>(_telemetryClient));
    }
}
