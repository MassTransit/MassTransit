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
namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Topology;
    using Transports;


    public abstract class HostConfiguration<THost> :
        IHostConfiguration
        where THost : IBusHostControl
    {
        readonly THost _host;

        protected HostConfiguration(THost host)
        {
            _host = host;

            HostAddress = host.Address;
            Topology = host.Topology;
        }

        IBusHostControl IHostConfiguration.Host => _host;

        public Uri HostAddress { get; }
        public IHostTopology Topology { get; }

        public abstract bool Matches(Uri address);

        public abstract Task<ISendTransport> CreateSendTransport(Uri address);
    }
}