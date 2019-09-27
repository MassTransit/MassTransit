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
namespace MassTransit
{
    using System;
    using ApplicationInsights.Configuration;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;


    public static class ApplicationInsightsMiddlewareConfiguratorExtensions
    {
        /// <summary>
        /// Add support for ApplicationInsights to the pipeline, which will be used to track all consumer message reception.
        /// </summary>
        public static void UseApplicationInsightsOnConsume(this IConsumePipeConfigurator configurator,
            TelemetryClient telemetryClient, Action<IOperationHolder<RequestTelemetry>, ConsumeContext> configureOperation = null,
            string telemetryHeaderRootKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderRootKey,
            string telemetryHeaderParentKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderParentKey)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (telemetryClient == null)
                throw new ArgumentNullException(nameof(telemetryClient));

            var observer = new TelemetryConsumeConfigurationObserver(configurator, telemetryClient, telemetryHeaderRootKey,
                telemetryHeaderParentKey, configureOperation);
        }

        /// <summary>
        /// Add support for ApplicationInsight to track all send message on the bus.
        /// </summary>
        public static void UseApplicationInsightsOnSend(this ISendPipelineConfigurator configurator,
            TelemetryClient telemetryClient,
            Action<IOperationHolder<DependencyTelemetry>, SendContext> configureOperation = null,
            string telemetryHeaderRootKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderRootKey,
            string telemetryHeaderParentKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderParentKey)
        {
            configurator.ConfigureSend(x => x.ConnectSendPipeSpecificationObserver(new TelemetrySendPipeSpecificationObserver(telemetryClient, telemetryHeaderRootKey,
                telemetryHeaderParentKey, configureOperation)));
        }

        /// <summary>
        /// Add support for ApplicationInsights to the pipeline, which will be used to track all message publication.
        /// </summary>
        public static void UseApplicationInsightsOnPublish(this IPublishPipelineConfigurator configurator,
            TelemetryClient telemetryClient,
            Action<IOperationHolder<DependencyTelemetry>, PublishContext> configureOperation = null,
            string telemetryHeaderRootKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderRootKey,
            string telemetryHeaderParentKey = ApplicationInsightsDefaultConfiguration.DefaultTelemetryHeaderParentKey)
        {
            configurator.ConfigurePublish(x => x.ConnectPublishPipeSpecificationObserver(new TelemetryPublishPipeSpecificationObserver(telemetryClient, telemetryHeaderRootKey,
                telemetryHeaderParentKey, configureOperation)));
        }
    }
}
