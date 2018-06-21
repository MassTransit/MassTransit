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
namespace MassTransit.MongoDbIntegration.Courier.Documents
{
    public class HostDocument
    {
        public HostDocument(HostInfo host)
        {
            MachineName = host.MachineName;
            ProcessId = host.ProcessId;
            ProcessName = host.ProcessName;
            Assembly = host.Assembly;
            AssemblyVersion = host.AssemblyVersion;
            MassTransitVersion = host.MassTransitVersion;
            FrameworkVersion = host.FrameworkVersion;
            OperatingSystemVersion = host.OperatingSystemVersion;
        }

        public string MachineName { get; private set; }
        public string ProcessName { get; private set; }
        public int ProcessId { get; private set; }
        public string Assembly { get; private set; }
        public string AssemblyVersion { get; private set; }
        public string FrameworkVersion { get; private set; }
        public string MassTransitVersion { get; private set; }
        public string OperatingSystemVersion { get; private set; }
    }
}