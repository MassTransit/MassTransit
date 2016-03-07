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
using MongoMessageDataRepository = MassTransit.MongoDbIntegration.MessageData.MongoMessageDataRepository;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests.MongoMessageDataRepositoryTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.GridFS;
    using NUnit.Framework;


    public class MongoMessageDataRepositoryTestsForGettingMessageData
    {
        GridFSBucket _bucket;
        byte[] _expected;
        Stream _result;
        Uri _uri;

        public void GivenAMongoMessageDataRepository_WhenGettingMessageData()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);

            var fixture = new Fixture();
            _expected = fixture.Create<byte[]>();
            _uri = fixture.Create<Uri>();

            var objectId = SeedBucket(_expected).GetAwaiter().GetResult();
            _resolver = new Mock<IMongoMessageUriResolver>();
            _resolver.Setup(m => m.Resolve(_uri)).Returns(objectId);

            var nameCreator = new Mock<IFileNameCreator>();
            nameCreator.Setup(m => m.CreateFileName()).Returns(fixture.Create<string>());

            var sut = new MongoMessageDataRepository(_resolver.Object, _bucket, nameCreator.Object);
            _result = sut.Get(_uri).GetAwaiter().GetResult();
        }

        [Test]
        public void ThenResolverCalledWithUri()
        {
            _resolver.Verify(m => m.Resolve(_uri));
        }

        [Test]
        public async Task ThenStreamReturnedAsExpected()
        {
            var result = new byte[_result.Length];
            await _result.ReadAsync(result, 0, result.Length);
            Assert.That(result, Is.EqualTo(_expected));
        }

        [OneTimeTearDown]
        public void Kill()
        {
            _bucket.DropAsync().GetAwaiter().GetResult();
        }

        async Task<ObjectId> SeedBucket(byte[] seed)
        {
            return await _bucket.UploadFromBytesAsync(Path.GetRandomFileName(), seed);
        }
    }
}