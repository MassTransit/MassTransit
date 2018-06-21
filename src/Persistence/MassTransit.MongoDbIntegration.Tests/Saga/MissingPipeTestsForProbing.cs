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
    using GreenPipes;
    using MongoDbIntegration.Saga;
    using MongoDbIntegration.Saga.Context;
    using MongoDbIntegration.Saga.Pipeline;
    using MongoDB.Driver;
    using Moq;
    using NUnit.Framework;
    using Pipeline;


    [TestFixture]
    public class MissingPipeTestsForProbing
    {
        [Test]
        public void ThenNextPipeProbed()
        {
            _nextPipe.Verify(m => m.Probe(_probeContext.Object), Times.Once);
        }

        [SetUp]
        public void WhenProbing()
        {
            _pipe.Probe(_probeContext.Object);
        }

        Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>> _nextPipe;
        MissingPipe<SimpleSaga, InitiateSimpleSaga> _pipe;
        Mock<ProbeContext> _probeContext;

        [OneTimeSetUp]
        public void GivenAMissingPipe()
        {
            _probeContext = new Mock<ProbeContext>();

            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>>();

            _pipe = new MissingPipe<SimpleSaga, InitiateSimpleSaga>(Mock.Of<IMongoCollection<SimpleSaga>>(), _nextPipe.Object,
                Mock.Of<IMongoDbSagaConsumeContextFactory>());
        }
    }
}