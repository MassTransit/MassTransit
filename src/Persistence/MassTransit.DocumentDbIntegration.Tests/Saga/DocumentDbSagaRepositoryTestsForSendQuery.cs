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
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using DocumentDbIntegration.Saga;
    using DocumentDbIntegration.Saga.Context;
    using DocumentDbIntegration.Saga.Pipeline;
    using Moq;
    using NUnit.Framework;
    using Pipeline;
    using Newtonsoft.Json;
    using Microsoft.Azure.Documents;

    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendQuery
    {
        [Test]
        public void ThenMissingPipeNotCalled()
        {
            _sagaPolicy.Verify(x => x.Missing(_sagaQueryConsumeContext.Object, It.IsAny<MissingPipe<SimpleSaga, InitiateSimpleSaga>>()), Times.Never);
        }

        [Test]
        public void ThenSagaSentToInstance()
        {
            _sagaPolicy.Verify(x => x.Existing(_sagaConsumeContext.Object, _nextPipe.Object));
        }

        [Test]
        public async Task ThenVersionIncremeted()
        {
            var sagaDocument = await SagaRepository.Instance.GetSagaDocument(_correlationId);

            var etagGuid = JsonConvert.DeserializeObject<Guid>(sagaDocument.ETag);
            Assert.That(etagGuid != Guid.Empty, Is.True);
        }

        Guid _correlationId;
        Mock<ISagaPolicy<SimpleSaga, InitiateSimpleSaga>> _sagaPolicy;
        Mock<SagaQueryConsumeContext<SimpleSaga, InitiateSimpleSaga>> _sagaQueryConsumeContext;
        Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>> _nextPipe;
        Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>> _sagaConsumeContext;
        Mock<IDocumentDbSagaConsumeContextFactory> _sagaConsumeContextFactory;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingQuery()
        {
            _correlationId = Guid.NewGuid();
            var saga = new SimpleSaga { CorrelationId = _correlationId };

            await SagaRepository.Instance.InsertSaga(saga);

            _sagaQueryConsumeContext = new Mock<SagaQueryConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _sagaQueryConsumeContext.Setup(x => x.Query.FilterExpression).Returns(x => x.CorrelationId == _correlationId);
            _sagaPolicy = new Mock<ISagaPolicy<SimpleSaga, InitiateSimpleSaga>>();
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>>();

            _sagaConsumeContext = new Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _sagaConsumeContext.Setup(x => x.CorrelationId).Returns(_correlationId);

            _sagaConsumeContextFactory = new Mock<IDocumentDbSagaConsumeContextFactory>();
            _sagaConsumeContextFactory.Setup(
                m =>
                    m.Create(It.IsAny<IDocumentClient>(), It.IsAny<string>(), It.IsAny<string>(), _sagaQueryConsumeContext.Object,
                        It.Is<SimpleSaga>(x => x.CorrelationId == _correlationId), true)).Returns(_sagaConsumeContext.Object);

            var repository = new DocumentDbSagaRepository<SimpleSaga>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, SagaRepository.CollectionName, _sagaConsumeContextFactory.Object);

            await repository.SendQuery(_sagaQueryConsumeContext.Object, _sagaPolicy.Object, _nextPipe.Object);
        }

        [OneTimeTearDown]
        public async Task Kill()
        {
            await SagaRepository.Instance.DeleteSaga(_correlationId);
        }
    }
}