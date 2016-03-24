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
namespace MassTransit.HttpTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Clients;
    using Hosting;


    public static class HttpAddressExtensions
    {
        public static HttpSendSettings GetSendSettings(this Uri uri)
        {
            return new HttpSendSettingsImpl(HttpMethod.Post, uri.LocalPath);
        }

        public static HttpHostSettings GetHostSettings(this Uri uri)
        {
            return new ConfigurationHostSettings(uri.Scheme, uri.Host, uri.Port, HttpMethod.Get);
        }

        public static Uri GetInputAddress(this HttpHostSettings hostSettings)
        {
            var builder = new UriBuilder
            {
                Scheme = hostSettings.Scheme,
                Host = hostSettings.Host,
                Port = hostSettings.Port,
            };

            return builder.Uri;
        }
        

        static IEnumerable<string> GetQueryStringOptions(HttpSendSettings settings)
        {
            return Enumerable.Empty<string>();
        }

        public static Uri GetSendAddress(this HttpHostSettings hostSettings, HttpSendSettings sendSettings)
        {
            var builder = new UriBuilder
            {
                Scheme = "http",
                Host = hostSettings.Host,
                Port = hostSettings.Port,
                Path = sendSettings.Path
            };

            builder.Query += string.Join("&", GetQueryStringOptions(sendSettings));

            return builder.Uri;
        }
    }
}