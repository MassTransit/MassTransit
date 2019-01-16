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

#if NETSTANDARD
namespace MassTransit.PublishPipeSpecifications
{
    using System.Diagnostics;
    using PipeConfigurators;

    public class ActivityPublishPipeSpecificationObserver : IPublishPipeSpecificationObserver
    {
        readonly DiagnosticSource _diagnosticSource;
        readonly string _activityIdKey;
        readonly string _activityCorrelationContextKey;

        public ActivityPublishPipeSpecificationObserver(DiagnosticSource diagnosticSource, string activityIdKey, string activityCorrelationContextKey)
        {
            _diagnosticSource = diagnosticSource;
            _activityIdKey = activityIdKey;
            _activityCorrelationContextKey = activityCorrelationContextKey;
        }
        void IPublishPipeSpecificationObserver.MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
        {
            var applicationInsightsPublishSpecification =
                new DiagnosticsActivitySendPipeSpecification<T>(_diagnosticSource, _activityIdKey, _activityCorrelationContextKey);

            specification.AddPipeSpecification(applicationInsightsPublishSpecification);
        }
    }
}
#endif