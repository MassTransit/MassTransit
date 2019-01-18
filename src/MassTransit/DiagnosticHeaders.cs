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

namespace MassTransit
{
    public static class DiagnosticHeaders
    {
        public const string ActivityIdHeader = "MT-Activity-ID";
        public const string ActivityCorrelationContext = "MT-Activity-Correlation-Context";

        public const string MessageId = "message-id";
        public const string CorrelationId = "correlation-id";
        public const string InitiatorId = "initiator-id";
        public const string SourceAddress = "source-address";
        public const string DestinationAddress = "destination-address";
        public const string InputAddress = "input-address";
        public const string CorrelationConversationId = "correlation-conversation-id";

        public const string SourceHostMachine = "source-host-machine";
        public const string SourceHostProcessId = "source-host-process-id";
        public const string SourceHostFrameworkVersion = "source-host-framework-version";
        public const string SourceHostMassTransitVersion = "source-host-masstransit-version";
        public const string MessageTypes = "message-types";
    }
}
