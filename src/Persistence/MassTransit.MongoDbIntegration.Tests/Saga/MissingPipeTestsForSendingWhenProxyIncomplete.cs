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
namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MongoDbIntegration.Saga;
    using MongoDbIntegration.Saga.Context;
    using MongoDbIntegration.Saga.Pipeline;
    using MongoDB.Driver;
    using Moq;
    using NUnit.Framework;
    using Pipeline;
    using Util;


    [TestFixture]
    public class MissingPipeTestsForSendingWhenProxyIncomplete
    {
        [Test]
        public void ThenNextPipeCalled()
        {
            _nextPipe.Verify(m => m.Send(It.IsAny<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>()), Times.Once);
        }

        [Test]
        public async Task ThenSagaInsertedIntoMongo()
        {
            var saga = await SagaRepository.GetSaga(_saga.CorrelationId);

            Assert.That(saga, Is.Not.Null);
        }

        Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>> _nextPipe;
        MissingPipe<SimpleSaga, InitiateSimpleSaga> _pipe;
        Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>> _context;
        Mock<IMongoDbSagaConsumeContextFactory> _consumeContextFactory;
        Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>> _proxy;
        SimpleSaga _saga;

        [OneTimeSetUp]
        public async Task GivenAMissingPipe_WhenSendingAndProxyIncomplete()
        {
            IMongoCollection<SimpleSaga> collection = SagaRepository.Instance.GetCollection<SimpleSaga>("sagas");
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>>();
            _proxy = new Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _proxy.SetupGet(m => m.IsCompleted).Returns(false);
            _consumeContextFactory = new Mock<IMongoDbSagaConsumeContextFactory>();
            _saga = new SimpleSaga {CorrelationId = Guid.NewGuid()};
            _context = new Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _context.SetupGet(m => m.Saga).Returns(_saga);
            _consumeContextFactory.Setup(m => m.Create(collection, _context.Object, _context.Object.Saga, false)).Returns(_proxy.Object);

            _pipe = new MissingPipe<SimpleSaga, InitiateSimpleSaga>(collection, _nextPipe.Object, _consumeContextFactory.Object);

            await _pipe.Send(_context.Object);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.DeleteSaga(_saga.CorrelationId);
        }
    }
}