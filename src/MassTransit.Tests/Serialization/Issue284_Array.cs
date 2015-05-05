// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Serialization
{
    using System.Runtime.Serialization;
    using System.Threading;
    using NUnit.Framework;


    [TestFixture]
    public class Issue284_Array
    {
        [Test]
        public void VerifyIssue284()
        {
            IServiceBus loopbackBus = ServiceBusFactory.New(sbc =>
            {
                sbc.ReceiveFrom("loopback://localhost/Issue284");
                sbc.UseJsonSerializer();
            });

            var expected = new ContainerWithObjectArray()
            {
                ObjectArray = null,
            };

            var signal = new AutoResetEvent(false);
            ContainerWithObjectArray actual = null;

            loopbackBus.SubscribeHandler<ContainerWithObjectArray>(container =>
            {
                actual = container;
                signal.Set();
            });

            loopbackBus.Publish(expected);

            signal.WaitOne();

            Assert.IsNotNull(actual);
            Assert.IsNull(actual.ObjectArray); // This is where it fails.
        }

        // this fixes it without touching a thing
        //[JsonObject]
        public sealed class ContainerWithObjectArray : ISerializable
        {
            public ContainerWithObjectArray()
            {
            }

            public ContainerWithObjectArray(SerializationInfo info,
                StreamingContext context)
            {
//                ObjectArray = ((IList<object>)info.GetValue("ObjectArray",
//                    typeof(IList<object>))) // this fails with null, every time.ToArray();

                // with the ListJsonConverter fix for null, this now works as well.

                // this is the proper way to deserialize an array, and no need to check for Null before calling ToArray()
                ObjectArray = (object[])info.GetValue("ObjectArray", typeof(object[]));
            }

            public object[] ObjectArray { get; set; }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("ObjectArray", ObjectArray);
            }
        }
    }
}