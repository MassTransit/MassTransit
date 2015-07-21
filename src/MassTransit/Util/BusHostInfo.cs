// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util
{
    using System;
    using System.Diagnostics;
    using System.Reflection;


    class BusHostInfo :
        HostInfo
    {
        public BusHostInfo()
        {
            MachineName = Environment.MachineName;
            MassTransitVersion = typeof(IBus).Assembly.GetName().Version.ToString();
            FrameworkVersion = Environment.Version.ToString();
            OperatingSystemVersion = Environment.OSVersion.ToString();
            Process currentProcess = Process.GetCurrentProcess();
            ProcessId = currentProcess.Id;
            ProcessName = currentProcess.ProcessName;

            Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();
            AssemblyName assemblyName = entryAssembly.GetName();
            Assembly = assemblyName.Name;
            AssemblyVersion = assemblyName.Version.ToString();
        }

        public string MachineName { get; }
        public string ProcessName { get; }
        public int ProcessId { get; }
        public string Assembly { get; }
        public string AssemblyVersion { get; }
        public string FrameworkVersion { get; }
        public string MassTransitVersion { get; }
        public string OperatingSystemVersion { get; }
    }
}