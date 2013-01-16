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
    public class When_interoperating_with_the_guid_type
    {
        [Test]
        public void Should_convert_from_a_guid_quickly()
        {
            Guid g = Guid.NewGuid();

            NewId n = g.ToNewId();

            string ns = n.ToString();
            string gs = g.ToString();

            Assert.AreEqual(ns, gs);
        }

        [Test]
        public void Should_convert_to_guid_quickly()
        {
            NewId n = NewId.Next();

            Guid g = n.ToGuid();

            string ns = n.ToString();
            string gs = g.ToString();

            Assert.AreEqual(ns, gs);
        }

        [Test]
        public void Should_display_sequentially_for_newid()
        {
            NewId id = NewId.Next();

            Console.WriteLine(id.ToString("DS"));
        }

        [Test]
        public void Should_make_the_round_trip_successfully_via_bytes()
        {
            Guid g = Guid.NewGuid();

            var n = new NewId(g.ToByteArray());

            var gn = new Guid(n.ToByteArray());

            Assert.AreEqual(g, gn);
        }

        [Test]
        public void Should_match_string_output_b()
        {
            var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 14, 15};

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            string gs = g.ToString("B");
            string ns = n.ToString("B");

            Assert.AreEqual(gs, ns);
        }

        [Test]
        public void Should_match_string_output_d()
        {
            var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 14, 15};

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            string gs = g.ToString("d");
            string ns = n.ToString("d");

            Assert.AreEqual(gs, ns);
        }

        [Test]
        public void Should_match_string_output_n()
        {
            var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 14, 15};

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            string gs = g.ToString("N");
            string ns = n.ToString("N");

            Assert.AreEqual(gs, ns);
        }

        [Test]
        public void Should_match_string_output_p()
        {
            var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 14, 15};

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            string gs = g.ToString("P");
            string ns = n.ToString("P");

            Assert.AreEqual(gs, ns);
        }

        [Test]
        public void Should_properly_handle_string_passthrough()
        {
            NewId n = NewId.Next();

            string ns = n.ToString("D");

            var g = new Guid(ns);

            var nn = new NewId(g.ToString("D"));

            Assert.AreEqual(n, nn);
        }

        [Test]
        public void Should_support_the_same_constructor()
        {
            var guid = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            var newid = new NewId(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);

            Assert.AreEqual(guid.ToString(), newid.ToString());
        }

        [Test]
        public void Should_work_from_newid_to_guid_to_newid()
        {
            NewId n = NewId.Next();

            var g = new Guid(n.ToByteArray());

            var ng = new NewId(g.ToByteArray());

            Console.WriteLine(g.ToString("D"));

            Assert.AreEqual(n, ng);
        }
    }
}