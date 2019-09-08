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
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using GreenPipes;
    using MassTransit.Saga;
    using DocumentDbIntegration.Saga;
    using DocumentDbIntegration.Saga.Context;
    using Messages;
    using Moq;
    using NUnit.Framework;
    using Pipeline;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;
    using Microsoft.Azure.Documents.Client;

    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendingWhenPolicyReturnsInstance
    {
        [Test]
        public void ThenPolicyUpdatedWithSagaInstance()
        {
            _policy.Verify(m => m.Existing(_sagaConsumeContext.Object, _nextPipe.Object));
        }

        [Test]
        public void ThenPreInsertInstanceCalledToGetInstance()
        {
            _policy.Verify(m => m.PreInsertInstance(_context.Object, out _simpleSaga));
        }

        [Test]
        public async Task ThenSagaInstanceStored()
        {
            Assert.That(await SagaRepository.Instance.GetSaga<SimpleSagaResource>(_correlationId, true), Is.Not.Null);
        }

        [Test]
        public async Task ThenVersionIncremeted()
        {
            var sagaDocument = await SagaRepository.Instance.GetSagaDocument(_correlationId);

            var etagGuid = JsonConvert.DeserializeObject<Guid>(sagaDocument.ETag);
            Assert.That(etagGuid != Guid.Empty, Is.True);
        }

        Mock<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>> _policy;
        Mock<ConsumeContext<InitiateSimpleSaga>> _context;
        SimpleSagaResource _simpleSaga;
        Guid _correlationId;
        CancellationToken _cancellationToken;
        Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>> _nextPipe;
        Mock<IDocumentDbSagaConsumeContextFactory> _sagaConsumeContextFactory;
        Mock<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>> _sagaConsumeContext;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingAndPolicyReturnsInstance()
        {
            _correlationId = Guid.NewGuid();
            _cancellationToken = new CancellationToken();

            _context = new Mock<ConsumeContext<InitiateSimpleSaga>>();
            _context.Setup(x => x.CorrelationId).Returns(_correlationId);
            _context.Setup(m => m.CancellationToken).Returns(_cancellationToken);

            _simpleSaga = new SimpleSagaResource { CorrelationId = _correlationId };

            _policy = new Mock<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>>();
            _policy.Setup(x => x.PreInsertInstance(_context.Object, out _simpleSaga)).Returns(true);

            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>>();

            _sagaConsumeContext = new Mock<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>();
            _sagaConsumeContext.Setup(x => x.CorrelationId).Returns(_correlationId);

            _sagaConsumeContextFactory = new Mock<IDocumentDbSagaConsumeContextFactory>();
            _sagaConsumeContextFactory.Setup(m => m.Create(It.IsAny<IDocumentClient>(), It.IsAny<string>(), It.IsAny<string>(), _context.Object, It.IsAny<SimpleSagaResource>(), It.IsAny<bool>(), It.IsAny<RequestOptions>())).Returns(
                _sagaConsumeContext.Object);


            var repository = new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, SagaRepository.CollectionName, _sagaConsumeContextFactory.Object, null);

            await repository.Send(_context.Object, _policy.Object, _nextPipe.Object);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.Instance.DeleteSaga(_correlationId);
        }
    }
}