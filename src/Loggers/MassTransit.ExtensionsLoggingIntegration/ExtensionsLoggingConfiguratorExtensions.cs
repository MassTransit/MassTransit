// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Microsoft.Extensions.Logging;

    public static class ExtensionsLoggingConfiguratorExtensions
    {
        /// <summary> Configure the Mass Transit Service Bus to use Microsoft.Extensions.Logging. </summary>
        /// <param name="configurator"> The configurator to act on. </param>
        /// <param name="loggerFactory"> (Optional) The ILoggerFactory. If none supplied, will be created. </param>
        public static ILoggerFactory UseExtensionsLogging(this IBusFactoryConfigurator configurator, ILoggerFactory loggerFactory = null)
        {
            if (configurator == null)
            {
                throw new ArgumentNullException("configurator");
            }

            return ExtensionsLoggingIntegration.ExtensionsLogger.Use(loggerFactory);
        }
    }
}