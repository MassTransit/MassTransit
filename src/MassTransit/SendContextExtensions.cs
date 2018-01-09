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
    using System;
    using Util;


    public static class SendContextExtensions
    {
        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="headers"></param>
        public static void SetHostHeaders(this SendHeaders headers)
        {
            headers.Set("MT-Host-MachineName", HostMetadataCache.Host.MachineName);
            headers.Set("MT-Host-ProcessName", HostMetadataCache.Host.ProcessName);
            headers.Set("MT-Host-ProcessId", HostMetadataCache.Host.ProcessId.ToString("F0"));
            headers.Set("MT-Host-Assembly", HostMetadataCache.Host.Assembly);
            headers.Set("MT-Host-AssemblyVersion", HostMetadataCache.Host.AssemblyVersion);
            headers.Set("MT-Host-MassTransitVersion", HostMetadataCache.Host.MassTransitVersion);
            headers.Set("MT-Host-FrameworkVersion", HostMetadataCache.Host.FrameworkVersion);
            headers.Set("MT-Host-OperatingSystemVersion", HostMetadataCache.Host.OperatingSystemVersion);
        }

        /// <summary>
        /// Set the host headers on the SendContext (for error, dead-letter, etc.)
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="exception"></param>
        /// <param name="exceptionTimestamp"></param>
        public static void SetExceptionHeaders(this SendHeaders headers, Exception exception, DateTime exceptionTimestamp)
        {
            exception = exception.GetBaseException() ?? exception;

            var exceptionMessage = ExceptionUtil.GetMessage(exception);

            headers.Set(MessageHeaders.Reason, "fault");

            headers.Set(MessageHeaders.FaultMessage, exceptionMessage);
            headers.Set(MessageHeaders.FaultTimestamp, exceptionTimestamp.ToString("O"));
            headers.Set(MessageHeaders.FaultStackTrace, ExceptionUtil.GetStackTrace(exception));
        }
    }
}