// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Data;
    using DocumentDbIntegration.Saga;
    using GreenPipes;
    using MassTransit.Saga;
    using Messages;
    using Microsoft.Azure.Documents;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForSendingWhenCorrelationIdNull
    {
        [Test]
        public void ThenSagaExceptionThrown()
        {
            Assert.That(_exception, Is.Not.Null);
        }

        SagaException _exception;

        [OneTimeSetUp]
        public async Task GivenADocumentDbSagaRepository_WhenSendingWithNullCorrelationId()
        {
            var context = new Mock<ConsumeContext<InitiateSimpleSaga>>();
            context.Setup(x => x.CorrelationId).Returns(default(Guid?));

            var repository = new DocumentDbSagaRepository<SimpleSagaResource>(Mock.Of<IDocumentClient>(), "sagaTest");

            try
            {
                await repository.Send(context.Object, Mock.Of<ISagaPolicy<SimpleSagaResource, InitiateSimpleSaga>>(),
                    Mock.Of<IPipe<SagaConsumeContext<SimpleSagaResource, InitiateSimpleSaga>>>());
            }
            catch (SagaException exception)
            {
                _exception = exception;
            }
        }
    }
}