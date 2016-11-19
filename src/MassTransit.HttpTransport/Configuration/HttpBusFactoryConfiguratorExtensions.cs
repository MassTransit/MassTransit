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
    using HttpTransport;
    using HttpTransport.Configuration;


    public static class HttpBusFactoryConfiguratorExtensions
    {
        public static IHttpHost Host(this IHttpBusFactoryConfigurator cfg, Uri host, Action<IHttpHostConfigurator> configure = null)
        {
            return cfg.Host(host.Scheme, host.Host, host.Port, configure);
        }

        public static IHttpHost Host(this IHttpBusFactoryConfigurator cfg, string scheme, string host, int port,
            Action<IHttpHostConfigurator> configure = null)
        {
            var httpHostCfg = new HttpHostConfigurator(scheme, host, port);

            configure?.Invoke(httpHostCfg);

            return cfg.Host(httpHostCfg.Settings);
        }
    }
}