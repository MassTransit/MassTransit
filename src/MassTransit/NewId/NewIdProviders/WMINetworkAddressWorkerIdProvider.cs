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
    using System.Globalization;
    using System.Linq;
    using System.Management;


    public class WmiNetworkAddressWorkerIdProvider :
        IWorkerIdProvider
    {
        public byte[] GetWorkerId(int index)
        {
            return GetNetworkAddress(index);
        }

        static byte[] GetNetworkAddress(int index)
        {
            byte[] network = GetManagementObjects()
                .Skip(index)
                .FirstOrDefault();

            if (network == null)
                throw new InvalidOperationException("Unable to find usable network adapter for unique address");

            return network;
        }

        static IEnumerable<byte[]> GetManagementObjects()
        {
            var options = new EnumerationOptions {Rewindable = false, ReturnImmediately = true};
            var scope = new ManagementScope(ManagementPath.DefaultPath);
            var query = new ObjectQuery("SELECT * FROM Win32_NetworkAdapter");

            var searcher = new ManagementObjectSearcher(scope, query, options);
            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject obj in collection)
            {
                byte[] bytes;
                try
                {
                    PropertyData propertyData = obj.Properties["MACAddress"];
                    string propertyValue = propertyData.Value.ToString();

                    bytes = propertyValue.Split(':')
                                         .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                                         .ToArray();
                }
                catch (Exception)
                {
                    continue;
                }

                if (bytes.Length == 6)
                    yield return bytes;
            }
        }
    }
}