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
    using Data;
    using GreenPipes;
    using DocumentDbIntegration.Saga.Context;
    using DocumentDbIntegration.Saga.Pipeline;
    using Messages;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Moq;
    using NUnit.Framework;
    using Pipeline;
    using Util;


    [TestFixture]
    public class MissingPipeTestsForSendingWhenProxyCompleted
    {
        [Test]
        public void ThenNextPipeCalled()
        {
            _nextPipe.Verify(m => m.Send(It.IsAny<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>()), Times.Once);
        }

        [Test]
        public void ThenSagaNotInsertedIntoCollection()
        {
            _mockDocumentClient.Verify(m => m.CreateDocumentAsync(It.IsAny<Uri>(), It.IsAny<SimpleSaga>(), null, false, CancellationToken.None), Times.Never);
        }

        Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>> _nextPipe;
        MissingPipe<SimpleSaga, InitiateSimpleSaga> _pipe;
        Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>> _context;
        Mock<IDocumentClient> _mockDocumentClient;
        Mock<IDocumentDbSagaConsumeContextFactory> _consumeContextFactory;
        Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>> _proxy;

        [OneTimeSetUp]
        public void GivenAMissingPipe_WhenSendingAndProxyCompleted()
        {
            _nextPipe = new Mock<IPipe<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>>();
            _proxy = new Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _proxy.SetupGet(m => m.IsCompleted).Returns(true);
            _consumeContextFactory = new Mock<IDocumentDbSagaConsumeContextFactory>();
            _mockDocumentClient = new Mock<IDocumentClient>();
            _context = new Mock<SagaConsumeContext<SimpleSaga, InitiateSimpleSaga>>();
            _consumeContextFactory.Setup(m => m.Create(_mockDocumentClient.Object, "","", _context.Object, It.IsAny<SimpleSaga>(), false, null)).Returns(_proxy.Object);

            _pipe = new MissingPipe<SimpleSaga, InitiateSimpleSaga>(_mockDocumentClient.Object, "","", _nextPipe.Object, _consumeContextFactory.Object, null);

            TaskUtil.Await(() => _pipe.Send(_context.Object));
        }
    }
}