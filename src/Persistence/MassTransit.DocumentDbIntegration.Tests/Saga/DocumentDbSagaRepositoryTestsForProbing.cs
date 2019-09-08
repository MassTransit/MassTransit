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
    using Data;
    using GreenPipes;
    using DocumentDbIntegration.Saga;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class DocumentDbSagaRepositoryTestsForProbing
    {
        [Test]
        public void ThenScopeIsReturned()
        {
            _probeContext.Verify(m => m.CreateScope("sagaRepository"));
        }

        [Test]
        public void ThenScopeIsSet()
        {
            _scope.Verify(x => x.Set(It.IsAny<object>()));
        }

        Mock<ProbeContext> _probeContext;
        Mock<ProbeContext> _scope;

        [OneTimeSetUp]
        public void GivenADocumentDbSagaRepository_WhenProbing()
        {
            _scope = new Mock<ProbeContext>();

            _probeContext = new Mock<ProbeContext>();
            _probeContext.Setup(m => m.CreateScope("sagaRepository")).Returns(_scope.Object);

            var repository = new DocumentDbSagaRepository<SimpleSagaResource>(SagaRepository.Instance.Client, SagaRepository.DatabaseName);

            repository.Probe(_probeContext.Object);
        }
    }
}