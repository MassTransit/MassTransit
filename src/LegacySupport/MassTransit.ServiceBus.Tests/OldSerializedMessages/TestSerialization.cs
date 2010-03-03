// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.LegacySupport.Tests.OldSerializedMessages
{
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using SerializationCustomization;
    using Subscriptions.Messages;

    [TestFixture]
    public class TestSerialization
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            LegacySubscriptionProxyService.SetupAssemblyRedirectForOldMessages();
            Old = new BinaryFormatter();
            NewReader = LegacyBinaryFormatterBuilder.BuildReader();
            NewWriter = LegacyBinaryFormatterBuilder.BuildWriter();
            PlainFormatter = new BinaryFormatter();
            Factory = new OldMessageFactory();
        }

        public BinaryFormatter PlainFormatter { get; private set; }
        public BinaryFormatter NewReader { get; private set; }
        public BinaryFormatter NewWriter { get; private set; }
        public BinaryFormatter Old { get; private set; }
        public OldMessageFactory Factory { get; private set; }
    }
}