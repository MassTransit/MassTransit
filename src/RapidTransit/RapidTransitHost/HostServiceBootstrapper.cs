// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace RapidTransit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using MassTransit;
    using MassTransit.Internals.Extensions;
    using MassTransit.Util;
    using Topshelf.Logging;
    using Topshelf.Runtime;


    class HostServiceBootstrapper :
        TopshelfServiceBootstrapper<HostServiceBootstrapper>
    {
        static readonly LogWriter _log = HostLogger.Get<HostServiceBootstrapper>();

        public HostServiceBootstrapper(HostSettings hostSettings)
            : base(hostSettings)
        {
        }

        internal static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                string assemblyDir = Path.GetDirectoryName(path);
                _log.Debug("Loading subscribers from path " + assemblyDir);
                return assemblyDir;
            }
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            List<string> assemblies = Directory.EnumerateFiles(baseDirectory, "*.dll", SearchOption.AllDirectories)
                .Where(x => !x.StartsWith("MassTransit.", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var registrations = assemblies
                .Select(Assembly.ReflectionOnlyLoadFrom)
                .SelectMany(x => x.GetTypes().Select(y => new {Assembly = x, Type = y}))
                .Where(x => x.Type.HasInterface<IConsumer>())
                .GroupBy(x => x.Assembly)
                .Select(x => new {Assembly = Assembly.Load(x.Key.GetName()), Types = x})
                .SelectMany(x => x.Types.Select(y =>
                {
                    _log.DebugFormat("Registering consumer type: {0}", TypeMetadataCache.ShortName(y.Type));

                    return builder.RegisterType(y.Type);
                }))
                .ToList();
        }
    }
}