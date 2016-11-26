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
namespace MassTransit.HttpTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Hosting;
    using Microsoft.Owin.Hosting;
    using Owin;
    using Util;


    public class HttpOwinHostContext :
        BasePipeContext,
        OwinHostContext,
        IDisposable
    {
        readonly StartOptions _options;
        readonly ITaskParticipant _participant;
        SortedDictionary<string, List<Endpoint>> _endpoints;
        IDisposable _owinHost;
        bool _started;

        public HttpOwinHostContext(HttpHostSettings settings, ITaskSupervisor supervisor)
            : this(settings, supervisor.CreateParticipant($"{TypeMetadataCache<HttpOwinHostContext>.ShortName} - {settings.ToDebugString()}"))
        {
        }

        HttpOwinHostContext(HttpHostSettings settings, ITaskParticipant participant)
            : base(new PayloadCache(), participant.StoppedToken)
        {
            HostSettings = settings;
            _participant = participant;

            _endpoints = new SortedDictionary<string, List<Endpoint>>(StringComparer.OrdinalIgnoreCase);

            _options = new StartOptions
            {
                Port = HostSettings.Port
            };

            _options.Urls.Add(HostSettings.Host);

            _participant.SetReady();
        }

        public void Dispose()
        {
            _owinHost?.Dispose();

            _participant.SetComplete();
        }

        public HttpHostSettings HostSettings { get; }

        public void StopHttpListener()
        {
            if (_started)
            {
                _owinHost?.Dispose();
            }
        }

        public void StartHost()
        {
            _owinHost = WebApp.Start(_options, app =>
            {
                lock (_endpoints)
                {
                    foreach (KeyValuePair<string, List<Endpoint>> endpoint in _endpoints.Reverse())
                    {
                        foreach (var handler in endpoint.Value)
                        {
                            app.Map(endpoint.Key, x => x.Use(handler.Handler.Handle));
                        }
                    }
                }
            });

            _started = true;
        }

        public void RegisterEndpointHandler(string pathMatch, HttpConsumerAction handler)
        {
            if (_started)
                throw new InvalidOperationException("The host has already been started, no additional endpoints may be added.");

            lock (_endpoints)
            {
                List<Endpoint> handlers;
                if (_endpoints.TryGetValue(pathMatch, out handlers))
                    handlers.Add(new Endpoint(pathMatch, handler));
                else
                    _endpoints.Add(pathMatch, new List<Endpoint> {new Endpoint(pathMatch, handler)});
            }
        }


        struct Endpoint
        {
            public readonly string PathMatch;
            public readonly HttpConsumerAction Handler;

            public Endpoint(string pathMatch, HttpConsumerAction handler)
            {
                PathMatch = pathMatch;
                Handler = handler;
            }
        }
    }
}