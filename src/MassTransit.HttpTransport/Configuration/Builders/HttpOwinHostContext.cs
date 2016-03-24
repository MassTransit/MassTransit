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
    using MassTransit.Pipeline;
    using Microsoft.Owin.Hosting;
    using Owin;
    using Util;


    public class HttpOwinHostContext :
        BasePipeContext,
        OwinHostContext,
        IDisposable
    {
        readonly ITaskParticipant _participant;
        IDisposable _owinHost;
        bool _started;

        public HttpOwinHostContext( HttpHostSettings settings, ITaskSupervisor supervisor)
            : this(settings, supervisor.CreateParticipant($"{TypeMetadataCache<HttpOwinHostContext>.ShortName} - {settings.ToDebugString()}"))
        {
        }

        HttpOwinHostContext( HttpHostSettings settings, ITaskParticipant participant)
            : base(new PayloadCache(), participant.StoppedToken)
        {
            HostSettings = settings;
            _participant = participant;
            _participant.SetReady();
        }

        public void Dispose()
        {
            _owinHost?.Dispose();

            _participant.SetComplete();
        }

        public HttpHostSettings HostSettings { get; set; }

        public void StartHttpListener(HttpConsumerAction controller)
        {
            if (_started)
                return;

            _started = true;
            var options = new StartOptions();
            options.Urls.Add(HostSettings.Host);
            options.Port = HostSettings.Port;

            _owinHost = WebApp.Start(options, app =>
            {
                app.Use(controller.Handle);
            });
        }
    }
}