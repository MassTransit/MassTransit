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
    using MassTransit.Saga;
    using DocumentDbIntegration.Saga;
    using DocumentDbIntegration.Saga.Context;
    using Messages;
    using Moq;
    using NUnit.Framework;
    using Util;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendingWhenPolicyReturnsCompletedInstance
    {
        [Test]
        public async Task ThenTheCompletedSagaIsNotUpdated()
        {
            var actual = await SagaRepository.Instance.GetSagaDocument(_correlationId);

            Assert.That(actual.ETag, Is.EqualTo(_simpleSagaDocument.ETag));
        }

        SimpleSagaResource _simpleSaga;
        Document _simpleSagaDocument;
        CancellationToken _cancellationToken;
        Guid _correlationId;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRespository_WhenSendingCompletedInstance()
        {
            _correlationId = Guid.NewGuid();
            _cancellationToken = new CancellationToken();

            var context = new Mock<ConsumeContext<CompleteSimpleSaga>>();
            context.Setup(x => x.CorrelationId).Returns(_correlationId);
            context.Setup(m => m.CancellationToken).Returns(_cancellationToken);

            _simpleSaga = new SimpleSagaResource
            {
                CorrelationId = _correlationId
            };
            await _simpleSaga.Consume(It.IsAny<ConsumeContext<CompleteSimpleSaga>>());
            await SagaRepository.Instance.InsertSaga(_simpleSaga, true);
            _simpleSagaDocument = await SagaRepository.Instance.GetSagaDocument(_simpleSaga.CorrelationId);

            var sagaConsumeContext = new Mock<SagaConsumeContext<SimpleSagaResource, CompleteSimpleSaga>>();
            sagaConsumeContext.SetupGet(x => x.IsCompleted).Returns(true);
            var documentDbSagaConsumeContextFactory = new Mock<IDocumentDbSagaConsumeContextFactory>();
            documentDbSagaConsumeContextFactory.Setup(x => x.Create(It.IsAny<IDocumentClient>(), It.IsAny<string>(), It.IsAny<string>(), context.Object, It.IsAny<SimpleSagaResource>(), true, It.IsAny<RequestOptions>()))
                .Returns(sagaConsumeContext.Object);
            var repository = new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, SagaRepository.CollectionName, documentDbSagaConsumeContextFactory.Object, null);

            await repository.Send(context.Object, Mock.Of<ISagaPolicy<SimpleSagaResource, CompleteSimpleSaga>>(), null);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.Instance.DeleteSaga(_correlationId);
        }
    }
}