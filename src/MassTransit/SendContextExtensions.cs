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
namespace MassTransit
{
    using Util;


    static class SendContextExtensions
    {
        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="sendContext"></param>
        public static void SetHostHeaders(this SendContext sendContext)
        {
            sendContext.Headers.Set("MT-Host-MachineName", HostMetadataCache.Host.MachineName);
            sendContext.Headers.Set("MT-Host-ProcessName", HostMetadataCache.Host.ProcessName);
            sendContext.Headers.Set("MT-Host-ProcessId", HostMetadataCache.Host.ProcessId.ToString("F0"));
            sendContext.Headers.Set("MT-Host-Assembly", HostMetadataCache.Host.Assembly);
            sendContext.Headers.Set("MT-Host-AssemblyVersion", HostMetadataCache.Host.AssemblyVersion);
            sendContext.Headers.Set("MT-Host-MassTransitVersion", HostMetadataCache.Host.MassTransitVersion);
            sendContext.Headers.Set("MT-Host-FrameworkVersion", HostMetadataCache.Host.FrameworkVersion);
            sendContext.Headers.Set("MT-Host-OperatingSystemVersion", HostMetadataCache.Host.OperatingSystemVersion);
        }
    }
}