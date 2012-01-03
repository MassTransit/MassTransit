// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.ZeroMq.Tests.Assumptions
{
    using System;
    using System.Text;
    using NUnit.Framework;
    using ZMQ;

    [TestFixture]
    public class Api_Play
    {
         
        [Test]
        public void PubSub()
        {
            using(var cxt = new Context())
            {
                var pub = cxt.Socket(SocketType.PUB);
                var sub = cxt.Socket(SocketType.SUB);

                sub.Subscribe("", Encoding.Unicode);
                sub.Connect("tcp://localhost:8100");

                pub.Bind("tcp://localhost:8100");


//                pub.Send("dru", Encoding.Unicode);
//
//                var msg = sub.Recv(Encoding.Unicode);
//                Console.WriteLine(msg);

                pub.Dispose();
                sub.Dispose();
            }
        }
    }
}