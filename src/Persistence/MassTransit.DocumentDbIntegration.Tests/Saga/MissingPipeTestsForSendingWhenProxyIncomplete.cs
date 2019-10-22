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
namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using DocumentDbIntegration.Saga.Context;
    using DocumentDbIntegration.Saga.Pipeline;
    using GreenPipes;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;
    using Data;
    using Messages;


    [TestFixture]
    public class MissingPipeTestsForSendingWhenProxyIncomplete
    {
        [Test]
        public void ThenNextPipeCalled()
        {
            _nextPipe.Verify(m => m.Send(It.IsAny<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>()), Times.Once);
        }

        [Test]
        public async Task ThenSagaInsertedIntoDocument()
        {
            var saga = await SagaRepository.Instance.GetSaga<SimpleSagaResource>(_saga.CorrelationId, true);

            Assert.That(saga, Is.Not.Null);
        }

        Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>> _nextPipe;
        MissingPipe<SimpleSagaResource, InitiateSimpleSaga> _pipe;
        Mock<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>> _context;
        Mock<IDocumentDbSagaConsumeContextFactory> _consumeContextFactory;
        Mock<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>> _proxy;
        SimpleSagaResource _saga;

        [OneTimeSetUp]
        public async Task GivenAMissingPipe_WhenSendingAndProxyIncomplete()
        {
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>>();
            _proxy = new Mock<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>();
            _proxy.SetupGet(m => m.IsCompleted).Returns(false);
            _consumeContextFactory = new Mock<IDocumentDbSagaConsumeContextFactory>();
            _saga = new SimpleSagaResource { CorrelationId = Guid.NewGuid() };
            _context = new Mock<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>();
            _context.SetupGet(m => m.Saga).Returns(_saga);
            _consumeContextFactory.Setup(m => m.Create(SagaRepository.Instance.Client, SagaRepository.DatabaseName, SagaRepository.CollectionName, _context.Object, It.IsAny<SimpleSagaResource>(), false, null)).Returns(_proxy.Object);

            _pipe = new MissingPipe<SimpleSagaResource, InitiateSimpleSaga>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, SagaRepository.CollectionName, _nextPipe.Object, _consumeContextFactory.Object, null);

            await _pipe.Send(_context.Object);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.Instance.DeleteSaga(_saga.CorrelationId);
        }
    }
}