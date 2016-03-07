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
    using MongoDbIntegration.Saga;
    using MongoDbIntegration.Saga.Context;
    using MongoDB.Driver;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class MongoDbSagaConsumeContextTestsForPopContext
    {
        [Test]
        public void ThenMongoContextReturnedAsSagaConsumeContext()
        {
            Assert.That(_context, Is.Not.Null);
        }

        SagaConsumeContext<SimpleSaga, InitiateSimpleSaga> _context;

        [TestFixtureSetUp]
        public void GivenAMongoDbSagaConsumeContext_WhenPoppingContext()
        {
            var mongoDbSagaConsumeContext = new MongoDbSagaConsumeContext<SimpleSaga, InitiateSimpleSaga>(Mock.Of<IMongoCollection<SimpleSaga>>(),
                Mock.Of<ConsumeContext<InitiateSimpleSaga>>(), Mock.Of<SimpleSaga>());

            _context = mongoDbSagaConsumeContext.PopContext<InitiateSimpleSaga>();
        }
    }
}