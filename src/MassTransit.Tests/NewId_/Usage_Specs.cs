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
namespace MassTransit.Tests.NewId_
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_new_id
    {
        [Test]
        public void Should_format_just_like_a_default_guid_formatter()
        {
            var newId = new NewId();

            Assert.AreEqual("00000000-0000-0000-0000-000000000000", newId.ToString());
        }

        [Test]
        public void Should_format_just_like_a_fancy_guid_formatter()
        {
            var newId = new NewId();

            Assert.AreEqual("{00000000-0000-0000-0000-000000000000}", newId.ToString("B"));
        }

        [Test]
        public void Should_format_just_like_a_narrow_guid_formatter()
        {
            var newId = new NewId();

            Assert.AreEqual("00000000000000000000000000000000", newId.ToString("N"));
        }

        [Test]
        public void Should_format_just_like_a_parenthesis_guid_formatter()
        {
            var newId = new NewId();

            Assert.AreEqual("(00000000-0000-0000-0000-000000000000)", newId.ToString("P"));
        }

        [Test]
        public void Should_work_from_guid_to_newid_to_guid()
        {
            Guid g = Guid.NewGuid();

            var n = new NewId(g.ToByteArray());

            string gs = g.ToString("d");
            string ns = n.ToString("d");

            Assert.AreEqual(gs, ns);
        }
    }
}