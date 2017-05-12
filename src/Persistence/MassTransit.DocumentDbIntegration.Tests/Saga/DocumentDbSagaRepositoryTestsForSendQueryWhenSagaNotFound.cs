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


    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendQueryWhenSagaNotFound
    {
        [Test]
        public void ThenMissingPipeCalled()
        {
            _sagaPolicy.Verify(x => x.Missing(_sagaQueryConsumeContext.Object, It.IsAny<MissingPipe<SimpleSaga, InitiateSimpleSaga>>()), Times.Once);
        }

        [Test]
        public void ThenSagaNotSentToInstance()
        {
            _sagaPolicy.Verify(x => x.Existing(It.IsAny<DocumentDbSagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>(), _nextPipe.Object), Times.Never);
        }

        Mock<ISagaPolicy<SimpleSaga, InitiateSimpleSaga>> _sagaPolicy;
        Mock<SagaQueryConsumeContext<SimpleSaga, InitiateSimpleSaga>> _sagaQueryConsumeContext;
        Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>> _nextPipe;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingQueryAndSagaNotFound()
        {
            _sagaQueryConsumeContext = new Mock<SagaQueryConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _sagaQueryConsumeContext.Setup(x => x.Query.FilterExpression).Returns(x => x.CorrelationId == Guid.NewGuid());
            _sagaPolicy = new Mock<ISagaPolicy<SimpleSaga, InitiateSimpleSaga>>();
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>>();

            var repository = new DocumentDbSagaRepository<SimpleSaga>(SagaRepository.Instance.Client, SagaRepository.DatabaseName, SagaRepository.CollectionName, null);

            await repository.SendQuery(_sagaQueryConsumeContext.Object, _sagaPolicy.Object, _nextPipe.Object);
        }
    }
}