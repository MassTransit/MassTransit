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
namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System;
    using Context;
    using Hosting;
    using Util;


    public class HttpOwinHostContext :
        BasePipeContext,
        OwinHostContext,
        IDisposable
    {
        ITaskParticipant _participant;

        public HttpOwinHostContext(OwinHostInstance host, HttpHostSettings settings, ITaskSupervisor supervisor)
            : this(host, settings, supervisor.CreateParticipant($"{TypeMetadataCache<HttpOwinHostContext>.ShortName} - {settings.ToDebugString()}"))
        {
        }

        HttpOwinHostContext(OwinHostInstance host, HttpHostSettings settings, ITaskParticipant participant)
            : base(new PayloadCache(), participant.StoppedToken)
        {
            HostSettings = settings;
            Instance = host;
            _participant = participant;
            _participant.SetReady();
        }

        public void Dispose()
        {
            Instance.Dispose();
            _participant.SetComplete();
        }

        public HttpHostSettings HostSettings { get; set; }
        public OwinHostInstance Instance { get; }
    }
}