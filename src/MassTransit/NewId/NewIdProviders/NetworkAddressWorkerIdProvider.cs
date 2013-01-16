// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.NewIdProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.NetworkInformation;


    public class NetworkAddressWorkerIdProvider :
        IWorkerIdProvider
    {
        public byte[] GetWorkerId(int index)
        {
            return GetNetworkAddress(index);
        }

        static byte[] GetNetworkAddress(int index)
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            IEnumerable<NetworkInterface> ethernet =
                interfaces.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
            IEnumerable<NetworkInterface> gigabit =
                interfaces.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet);
            IEnumerable<NetworkInterface> wireless =
                interfaces.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);

            NetworkInterface network = ethernet.Concat(gigabit).Concat(wireless)
                                               .Skip(index)
                                               .FirstOrDefault();

            if (network == null)
                throw new InvalidOperationException("Unable to find usable network adapter for unique address");

            byte[] address = network.GetPhysicalAddress().GetAddressBytes();
            return address;
        }
    }
}