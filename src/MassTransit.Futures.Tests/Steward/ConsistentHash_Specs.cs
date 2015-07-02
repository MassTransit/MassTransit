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
namespace MassTransit.Tests.Steward
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using MassTransit.Steward.Core.Distribution.ConsistentHashing;
    using NUnit.Framework;


    [TestFixture]
    public class Performance_Specs
    {
        [Test]
        public void FirstTestName()
        {
            var servers = new List<Server>();
            for (int i = 0; i < 1000; i++)
                servers.Add(new Server(i));

            var ch = new ConsistentHashDistributionStrategy<Server>(new Murmur3AUnsafe(), x => x.Key);
            ch.Init(servers);

            int search = 100000;

            Stopwatch timer = Stopwatch.StartNew();
            var ay1 = new SortedList<int, int>();
            for (int i = 0; i < search; i++)
            {
                int temp = ch[i.ToString()].Id;

                ay1[i] = temp;
            }
            timer.Stop();

            decimal throughput = Stopwatch.Frequency / ((decimal)timer.ElapsedTicks / search);
            Console.WriteLine("Throughput: {0:F0}/s", throughput);


            Stopwatch topologyTimer = Stopwatch.StartNew();
            for (int i = 1000; i < 2000; i++)
            {
                var server = new Server(i);
                servers.Add(server);
                ch.Add(server);
            }
            topologyTimer.Stop();
            throughput = Stopwatch.Frequency * 1000m / topologyTimer.ElapsedTicks;
            Console.WriteLine("Topology Change Throughput: {0:F0}/s", throughput);


            Stopwatch timer2 = Stopwatch.StartNew();
            var ay2 = new SortedList<int, int>();
            for (int i = 0; i < search; i++)
            {
                int temp = ch[i.ToString()].Id;

                ay2[i] = temp;
            }
            timer2.Stop();

            throughput = Stopwatch.Frequency / ((decimal)timer2.ElapsedTicks / search);
            Console.WriteLine("Throughput: {0:F0}/s", throughput);

            int diff = 0;
            for (int i = 0; i < search; i++)
            {
                if (ay1[i] != ay2[i])
                    diff++;
            }

            Console.WriteLine("Change In Distribution: {0}", diff);
        }

        [Test]
        public void Under_heavy_topology_change_throughput()
        {
            var servers = new List<Server>();
            for (int i = 0; i < 1000; i++)
                servers.Add(new Server(i));

            var ch = new ConsistentHashDistributionStrategy<Server>(new Murmur3AUnsafe(), x => x.Key);
            ch.Init(servers);

            int search = 100000;

            Stopwatch timer = Stopwatch.StartNew();
            var ay1 = new SortedList<int, int>();
            for (int i = 0; i < search; i++)
            {
                int temp = ch[i.ToString()].Id;

                ay1[i] = temp;
            }
            timer.Stop();

            decimal throughput = Stopwatch.Frequency / ((decimal)timer.ElapsedTicks / search);
            Console.WriteLine("Throughput: {0:F2}/s", throughput);


            Task topologyTask = Task.Factory.StartNew(() =>
            {
                Stopwatch topologyTimer = Stopwatch.StartNew();
                for (int i = 1000; i < 2000; i++)
                {
                    var server = new Server(i);
                    servers.Add(server);
                    ch.Add(server);
                }
                topologyTimer.Stop();
                throughput = Stopwatch.Frequency / ((decimal)topologyTimer.ElapsedTicks / 1000);
                Console.WriteLine("Topology Change Throughput: {0:F2}/s", throughput);
            }, TaskCreationOptions.LongRunning);

            var ay2 = new SortedList<int, int>();
            Task searchTask = Task.Factory.StartNew(() =>
            {
                Stopwatch timer2 = Stopwatch.StartNew();
                for (int i = 0; i < search; i++)
                {
                    int temp = ch[i.ToString()].Id;

                    ay2[i] = temp;
                }
                timer2.Stop();

                throughput = Stopwatch.Frequency / ((decimal)timer2.ElapsedTicks / search);
                Console.WriteLine("Throughput: {0:F2}/s", throughput);
            }, TaskCreationOptions.LongRunning);

            Task.WaitAll(topologyTask, searchTask);


            int diff = 0;
            for (int i = 0; i < search; i++)
            {
                if (ay1[i] != ay2[i])
                    diff++;
            }

            Console.WriteLine("Change In Distribution: {0}", diff);
        }


        class Server
        {
            string _key;

            public Server(int id)
            {
                Id = id;
                _key = string.Format("SERVER_{0}", id);
            }

            public string Key
            {
                get { return _key; }
            }

            public int Id { get; set; }

            public override string ToString()
            {
                return _key;
            }
        }
    }
}