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
    using Microsoft.Azure.Documents;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class DocumentDbSagaConsumeContextTestsForPopContext
    {
        [Test]
        public void ThenDocumentDbContextReturnedAsSagaConsumeContext()
        {
            Assert.That(_context, Is.Not.Null);
        }

        SagaConsumeContext<SimpleSaga, InitiateSimpleSaga> _context;

        [OneTimeSetUp]
        public void GivenADocumentDbSagaConsumeContext_WhenPoppingContext()
        {
            var mongoDbSagaConsumeContext = new DocumentDbSagaConsumeContext<SimpleSaga, InitiateSimpleSaga>(It.IsAny<IDocumentClient>(),It.IsAny<string>(), It.IsAny<string>(),
                Mock.Of<ConsumeContext<InitiateSimpleSaga>>(), Mock.Of<SimpleSaga>());

            _context = mongoDbSagaConsumeContext.PopContext<InitiateSimpleSaga>();
        }
    }
}