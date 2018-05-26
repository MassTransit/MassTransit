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
    using GreenPipes;
    using Microsoft.ApplicationInsights;


    public static class ApplicationInsightsMiddlewareConfiguratorExtensions
    {
        /// <summary>
        /// Add support for ApplicationInsights to the pipeline, which will be used to track all consumer message reception
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="telemetryClient"></param>
        /// <typeparam name="T"></typeparam>
        public static void UseApplicationInsights<T>(this IPipeConfigurator<T> configurator, TelemetryClient telemetryClient)
            where T : class, ConsumeContext
        {
            configurator.AddPipeSpecification(new ApplicationInsightsSpecification<T>(telemetryClient));
        }
    }
}