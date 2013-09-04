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
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;


    public class HostNameSHA1WorkerIdProvider :
        IWorkerIdProvider
    {
        public byte[] GetWorkerId(int index)
        {
            return GetNetworkAddress();
        }

        static byte[] GetNetworkAddress()
        {
            try
            {
                string hostName = Dns.GetHostName();

                SHA1 hasher = SHA1.Create();
                byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(hostName));

                var bytes = new byte[6];
                Buffer.BlockCopy(hash, 12, bytes, 0, 6);
                bytes[0] |= 0x80;

                return bytes;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve hostname", ex);
            }
        }
    }
}