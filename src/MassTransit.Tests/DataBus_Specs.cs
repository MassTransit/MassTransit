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
namespace MassTransit.Tests
{
    namespace DataBus_Specs
    {
        using System;
        using System.IO;
        using System.Text;
        using System.Threading;
        using MessageData;
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class Storing_large_message_data :
            InMemoryTestFixture
        {
            [Test]
            public async void Should_be_wildly_useful_for_big_stuff()
            {
                IMessageDataRepository storage = new InMemoryMessageDataRepository();

                var s = new string('*', 1000);

                Uri address = await storage.Put(new MemoryStream(Encoding.UTF8.GetBytes(s)));

                var property = new ReadStringMessageData(address, storage, default(CancellationToken));

                var message = new ReadBigMessage {Body = property};

                var value = await message.Body.Value;

                Assert.AreEqual(s, value);
            }
        }


        interface BigMessage
        {
            MessageData<string> Body { get; }
        }


        class ReadBigMessage :
            BigMessage
        {
            public MessageData<string> Body { get; set; }
        }
    }
}