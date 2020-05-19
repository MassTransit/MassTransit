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
namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using MessageData;
    using MongoDB.Bson;
    using NUnit.Framework;


    public class MongoUriResolverTestsForResolvingObjectId
    {
        Uri _expected;
        Uri _result;

        [SetUp]
        public void GivenAMongoMessageUriResolver_WhenResolvingAnObjectId()
        {
            var objectId = ObjectId.GenerateNewId();
            _expected = new Uri(string.Format($"urn:mongodb:gridfs:{objectId}"));
            var sut = new MessageDataResolver();

            _result = sut.GetAddress(objectId);
        }

        [Test]
        public void ThenUriFormattedCorrectly()
        {
            Assert.That(_result, Is.EqualTo(_expected));
        }
    }
}