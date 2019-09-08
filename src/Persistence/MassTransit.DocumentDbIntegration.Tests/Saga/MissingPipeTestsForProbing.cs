namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using Data;
    using DocumentDbIntegration.Saga.Context;
    using DocumentDbIntegration.Saga.Pipeline;
    using GreenPipes;
    using Messages;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Moq;
    using NUnit.Framework;


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

            _pipe = new MissingPipe<SimpleSaga, InitiateSimpleSaga>(Mock.Of<IDocumentClient>(), SagaRepository.DatabaseName, SagaRepository.CollectionName, _nextPipe.Object,
                Mock.Of<IDocumentDbSagaConsumeContextFactory>(), null);
        }
    }
}
