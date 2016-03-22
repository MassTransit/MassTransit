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
namespace MassTransit.HttpTransport.Configuration
{
    using System.Net.Http;
    using Hosting;


    public class HttpHostConfigurator : IHttpHostConfigurator
    {
        readonly HttpHostSettingsImpl _settings;

        public HttpHostConfigurator(string scheme, string host, int port)
        {
            _settings = new HttpHostSettingsImpl(scheme, host, port, HttpMethod.Post);
        }

        public HttpHostSettings Settings
        {
            get { return _settings; }
        }

        public void UseMethod(HttpMethod method)
        {
            _settings.Method = method;
        }
    }
}