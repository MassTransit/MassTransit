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
namespace MassTransit.Monitoring.Performance.StatsD
{
    using System.Net.Sockets;


    public class StatsDPerformanceCounter : IPerformanceCounter
    {
        readonly string _fullName;

        readonly UdpClient _client;

        public StatsDPerformanceCounter(StatsDConfiguration cfg, string category, string name, string instance)
        {
            _fullName = $"{category}.{name}.{instance}";
            _client = new UdpClient(cfg.Port);
        }
        public void Increment()
        {
            var payload = $"{_fullName}:1|c";
            byte[] datagram = System.Text.Encoding.UTF8.GetBytes(payload);
            var t = _client.SendAsync(datagram, datagram.Length);
        }

        public void IncrementBy(long val)
        {
            var payload = $"{_fullName}:{val}|c";
            byte[] datagram = System.Text.Encoding.UTF8.GetBytes(payload);
            var t = _client.SendAsync(datagram, datagram.Length);
        }

        public void Set(long val)
        {
            var payload = $"{_fullName}:{val}|g";
            byte[] datagram = System.Text.Encoding.UTF8.GetBytes(payload);
            var t = _client.SendAsync(datagram, datagram.Length);
        }

        public void Dispose()
        {
            _client.Close();
        }
    }
}