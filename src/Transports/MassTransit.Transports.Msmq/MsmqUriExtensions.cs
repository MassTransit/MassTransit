// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Net;

    public static class MsmqUriExtensions
    {
        /// <summary>
        /// The name of the queue in local format (.\private$\name)
        /// </summary>
        public static string GetLocalName(this Uri uri)
        {
            return @".\private$\" + uri.AbsolutePath.Substring(1);
        }

        /// <summary>
        /// The format name used to talk to MSMQ
        /// </summary>
        public static string GetInboundFormatName(this Uri uri)
        {
            string hostName = GetMsmqHostName(uri);

            if (IsIpAddress(hostName))
                return string.Format(@"FormatName:DIRECT=TCP:{0}\private$\{1}", hostName, uri.AbsolutePath.Substring(1));

            return string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, uri.AbsolutePath.Substring(1));
        }

        /// <summary>
        /// The format name used to talk to MSMQ
        /// </summary>
        public static Uri GetInboundUri(this Uri uri)
        {
            if (!IsMsmqMulticast(uri))
                return null;

            string hostName = GetMsmqHostName(uri);

            return new Uri(string.Format("msmq://{0}{1}", hostName, uri.PathAndQuery));
        }

        public static string GetMsmqHostName(this Uri uri)
        {
            if (IsMsmqMulticast(uri))
                return Environment.MachineName.ToLowerInvariant();

            return uri.Host;
        }
      
        public static IMsmqEndpointAddress GetQueueAddress(this Uri uri)
        {
            string hostName = GetMsmqHostName(uri);

            return new MsmqEndpointAddress(new Uri(new UriBuilder("msmq", hostName).Uri, uri.AbsolutePath));
        }	

        public static string GetOutboundFormatName(this Uri uri)
        {
            if (IsMsmqMulticast(uri))
            {
                IPAddress address = IPAddress.Parse(uri.Host);

                return string.Format(@"FormatName:MULTICAST={0}:{1}", address, uri.Port);
            }

            return GetInboundFormatName(uri);
        }

        public static string GetMulticastAddress(this Uri uri)
        {
            if (!IsMsmqMulticast(uri))
                return null;

            return string.Format("{0}:{1}", uri.Host, uri.Port == 0 ? 7784 : uri.Port);
        }

        private static bool IsMsmqMulticast(this Uri uri)
        {
            return uri.Scheme.ToLowerInvariant() == "msmq-pgm";
        }

        private static bool IsIpAddress(string hostName)
        {
            IPAddress address;
            return IPAddress.TryParse(hostName, out address);
        }
    }
}