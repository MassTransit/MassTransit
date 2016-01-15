// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Hosting
{
    using System;
    using MassTransit.Hosting;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public interface ServiceBusSettings :
        ISettings
    {
        string ConnectionString { get; }
        string Namespace { get; }
        string ServicePath { get; }

        string KeyName { get; }
        string SharedAccessKey { get; }
        TimeSpan? OperationTimeout { get; }
        TimeSpan? TokenTimeToLive { get; }
        TokenScope TokenScope { get; }
        TimeSpan? RetryMinBackoff { get; }
        TimeSpan? RetryMaxBackoff { get; }
        int? RetryLimit { get; }
        TransportType TransportType { get; }
    }
}