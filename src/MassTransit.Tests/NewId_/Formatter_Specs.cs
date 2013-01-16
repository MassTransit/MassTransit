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
    using NUnit.Framework;
    using NewIdFormatters;
    using NewIdParsers;


    [TestFixture]
    public class Using_the_newid_formatters
    {
        [Test]
        public void Should_convert_back_using_parser()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new ZBase32Formatter(true);

            string ns = n.ToString(formatter);

            var parser = new ZBase32Parser();
            NewId newId = parser.Parse(ns);


            Assert.AreEqual(n, newId);
        }

        [Test]
        public void Should_convert_back_using_standard_parser()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new Base32Formatter(true);

            string ns = n.ToString(formatter);

            var parser = new Base32Parser();
            NewId newId = parser.Parse(ns);


            Assert.AreEqual(n, newId);
        }

        [Test]
        public void Should_convert_using_custom_base32_formatting_characters()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new Base32Formatter("0123456789ABCDEFGHIJKLMNOPQRSTUV");

            string ns = n.ToString(formatter);

            Assert.AreEqual("UQP7OV4AN129HB4N79GGF8GJ10", ns);
        }

        [Test]
        public void Should_convert_using_standard_base32_formatting_characters()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new Base32Formatter(true);

            string ns = n.ToString(formatter);

            Assert.AreEqual("62ZHY7EKXBCJRLEXHJQQPIQTBA", ns);
        }

        [Test]
        public void Should_convert_using_the_optimized_human_readable_formatter()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new ZBase32Formatter(true);

            string ns = n.ToString(formatter);

            Assert.AreEqual("6438A9RKZBNJTMRZ8JOOXEOUBY", ns);
        }

        [Test]
        public void Should_translate_often_transposed_characters_to_proper_values()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            string ns = "6438A9RK2BNJTMRZ8J0OXE0UBY";

            var parser = new ZBase32Parser(true);
            NewId newId = parser.Parse(ns);


            Assert.AreEqual(n, newId);
        }
    }
}