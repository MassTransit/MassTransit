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
namespace MassTransit.WindowsServiceBusTransport.Tests
{
    public class TestServiceBusAccountSettings
    {
        public TestServiceBusAccountSettings()
        {
            ConnectionString = "Endpoint=sb://servicebusformt/ServiceBusDefaultNamespace;StsEndpoint=https://servicebusformt:9355/ServiceBusDefaultNamespace;RuntimePort=9354;ManagementPort=9355;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=txRMZ+zWDgr4ysHs91yvxdoTWoqcvfdleEqXAl2LzQ0=";
        }

        public string ConnectionString { get; }
    }
}