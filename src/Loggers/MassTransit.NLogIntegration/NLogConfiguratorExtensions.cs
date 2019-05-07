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
namespace MassTransit.NLogIntegration
{
    using Logging;
    using NLog;
    using NLog.Extensions.Logging;


    /// <summary>
    /// Extensions for configuring NLog with MassTransit
    /// </summary>
    public static class NLogConfiguratorExtensions
    {
        /// <summary>
        /// Specify that you want to use the NLog logging framework with MassTransit.
        /// </summary>
        /// <param name="configurator">Optional service bus configurator</param>
        public static void UseNLog(this IBusFactoryConfigurator configurator)
        {
            configurator.UseLoggerFactory(new NLogLoggerFactory());
        }

        /// <summary>
        /// Specify that you want to use the NLog logging framework with MassTransit.
        /// </summary>
        /// <param name="configurator">Optional service bus configurator</param>
        /// <param name="options">Options</param>
        public static void UseNLog(this IBusFactoryConfigurator configurator, NLogProviderOptions options)
        {
            configurator.UseLoggerFactory(new NLogLoggerFactory(options));
        }
    }
}
