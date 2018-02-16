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
namespace MassTransit.HttpTransport.Configuration
{
    using MassTransit.Configuration;
    using Transport;


    public class HttpHostConfiguration :
        HostConfiguration<HttpHost>,
        IHttpHostConfiguration
    {
        readonly IHttpBusConfiguration _busConfiguration;

        public HttpHostConfiguration(IHttpBusConfiguration busConfiguration, HttpHost host)
            : base(host)
        {
            Host = host;
            _busConfiguration = busConfiguration;
        }

        public IHttpReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string pathMatch)
        {
            return new HttpReceiveEndpointConfiguration(this, pathMatch, _busConfiguration.CreateEndpointConfiguration());
        }

        public IHttpBusConfiguration BusConfiguration => _busConfiguration;

        public HttpHost Host { get; }
    }
}