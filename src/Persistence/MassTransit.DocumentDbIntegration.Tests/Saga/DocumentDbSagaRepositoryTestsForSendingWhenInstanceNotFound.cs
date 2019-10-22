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
    using DocumentDbIntegration.Saga;
    using DocumentDbIntegration.Saga.Pipeline;
    using GreenPipes;
    using MassTransit.Saga;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using Messages;


    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendingWhenInstanceNotFound
    {
        [Test]
        public void ThenMissingPipeInvokedOnPolicy()
        {
            _policy.Verify(m => m.Missing(_context.Object, It.IsAny<MissingPipe<SimpleSagaResource, InitiateSimpleSaga>>()));
        }

        Mock<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>> _policy;
        Mock<ConsumeContext<InitiateSimpleSaga>> _context;
        SimpleSagaResource _nullSimpleSaga;
        Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>> _nextPipe;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingAndInstanceNotFound()
        {
            _context = new Mock<ConsumeContext<InitiateSimpleSaga>>();
            _context.Setup(x => x.CorrelationId).Returns(It.IsAny<Guid>());
            _context.Setup(m => m.CancellationToken).Returns(It.IsAny<CancellationToken>());

            _nullSimpleSaga = null;

            _policy = new Mock<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>>();
            _policy.Setup(x => x.PreInsertInstance(_context.Object, out _nullSimpleSaga)).Returns(false);

            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>>();

            var repository = new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName);

            await repository.Send(_context.Object, _policy.Object, _nextPipe.Object);
        }
    }
}