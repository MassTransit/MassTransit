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
#if NETSTANDARD
namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using GreenPipes;
    using Pipeline.Filters.DiagnosticActivity;


    public class DiagnosticsActivityPipeSpecification<T> :
        IPipeSpecification<SendContext<T>>,
        IPipeSpecification<PublishContext<T>>,
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly string _activityCorrelationContextKey;
        readonly string _activityIdKey;
        readonly DiagnosticSource _diagnosticSource;

        public DiagnosticsActivityPipeSpecification(DiagnosticSource diagnosticSource, string activityIdKey, string activityCorrelationContextKey)
        {
            _diagnosticSource = diagnosticSource;
            _activityIdKey = activityIdKey;
            _activityCorrelationContextKey = activityCorrelationContextKey;
        }

        public void Apply(IPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(new DiagnosticsActivitySendFilter<T>(_diagnosticSource, _activityIdKey, _activityCorrelationContextKey));
        }

        public void Apply(IPipeBuilder<PublishContext<T>> builder)
        {
            builder.AddFilter(new DiagnosticsActivityPublishFilter<T>(_diagnosticSource, _activityIdKey, _activityCorrelationContextKey));
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new DiagnosticsActivityConsumeFilter<T>(_diagnosticSource, _activityIdKey, _activityCorrelationContextKey));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_diagnosticSource == null)
                yield return this.Failure("Diagnostic Source should not be null");

            if (string.IsNullOrEmpty(_activityIdKey))
                yield return this.Failure("Diagnostic Activity Id Key should not be null");

            if (string.IsNullOrEmpty(_activityCorrelationContextKey))
                yield return this.Failure("Diagnostic Activity Correlation Context Key should not be null");
        }
    }
}
#endif
