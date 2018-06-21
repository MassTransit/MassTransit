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
namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using Util;


    [TestFixture]
    public class Sending_a_message_via_proxy
    {
        [Test]
        public void Should_property_initialize_the_values()
        {
            var values = new
            {
                CorrelationId = Guid.NewGuid(),
                Name = "Dru",
                Timestamp = DateTime.UtcNow,
            };

            IMessageType message = TypeMetadataCache<IMessageType>.InitializeFromObject(values);

           
            message.CorrelationId.ShouldBe(values.CorrelationId);
            message.Name.ShouldBe(values.Name);
            message.Timestamp.ShouldBe(values.Timestamp);
        }


        public interface IMessageType
        {
            Guid CorrelationId { get; }
            string Name { get; }
            DateTime Timestamp { get; }
        }
    }
}