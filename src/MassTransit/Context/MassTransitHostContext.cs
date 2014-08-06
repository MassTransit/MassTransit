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
namespace MassTransit.Context
{
    using System;
    using System.Diagnostics;
    using System.Reflection;


    public class MassTransitHostContext :
        HostContext
    {
        readonly string _assembly;
        readonly string _assemblyVersion;
        readonly string _frameworkVersion;
        readonly string _hostName;
        readonly string _massTransitVersion;
        readonly string _operatingSystemVersion;
        readonly int _processId;
        readonly string _processName;

        public MassTransitHostContext()
        {
            _hostName = Environment.MachineName;

            using (Process process = Process.GetCurrentProcess())
            {
                _processName = process.ProcessName;
                _processId = process.Id;
            }

            _massTransitVersion = typeof(IServiceBus).Assembly.GetName().Version.ToString();
            _frameworkVersion = Environment.Version.ToString();
            _operatingSystemVersion = Environment.OSVersion.VersionString;

            Assembly assembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            _assembly = assemblyName.Name;
            _assemblyVersion = assemblyName.Version.ToString();
        }

        public string AssemblyVersion
        {
            get { return _assemblyVersion; }
        }

        public string OperatingSystemVersion
        {
            get { return _operatingSystemVersion; }
        }

        public string Assembly
        {
            get { return _assembly; }
        }

        public string MassTransitVersion
        {
            get { return _massTransitVersion; }
        }

        public string FrameworkVersion
        {
            get { return _frameworkVersion; }
        }

        public string HostName
        {
            get { return _hostName; }
        }

        public string ProcessName
        {
            get { return _processName; }
        }

        public int ProcessId
        {
            get { return _processId; }
        }
    }
}