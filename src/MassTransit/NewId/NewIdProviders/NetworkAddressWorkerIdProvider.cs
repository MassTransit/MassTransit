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


    public class BestPossibleWorkerIdProvider :
        IWorkerIdProvider
    {
        public byte[] GetWorkerId(int index)
        {
            var exceptions = new List<Exception>();

            try
            {
                return new NetworkAddressWorkerIdProvider().GetWorkerId(index);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                return new WmiNetworkAddressWorkerIdProvider().GetWorkerId(index);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                return new HostNameSHA1WorkerIdProvider().GetWorkerId(index);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            throw new AggregateException(exceptions);
        }
    }


    public class NetworkAddressWorkerIdProvider :
        IWorkerIdProvider
    {
        public byte[] GetWorkerId(int index)
        {
            return GetNetworkAddress(index);
        }

        static byte[] GetNetworkAddress(int index)
        {
            byte[] network = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                            || x.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet
                            || x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                            || x.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx
                            || x.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT)
                .Select(x => x.GetPhysicalAddress())
                .Where(x => x != null)
                .Select(x => x.GetAddressBytes())
                .Where(x => x.Length == 6)
                .Skip(index)
                .FirstOrDefault();

            if (network == null)
                throw new InvalidOperationException("Unable to find usable network adapter for unique address");

            return network;
        }
    }
}