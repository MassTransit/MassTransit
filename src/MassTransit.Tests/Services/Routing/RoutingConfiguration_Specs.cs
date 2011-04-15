namespace MassTransit.Tests.Services.Routing
{
    using System;
    using MassTransit.Internal;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using MassTransit.Pipeline.Inspectors;
    using MassTransit.Services.Routing.Configuration;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class RoutingConfiguration_Specs
    {
        [SetUp]
        public void Setup_Context()
        {
            _builder = MockRepository.GenerateStub<IObjectBuilder>();
        	_bus = MockRepository.GenerateStub<IServiceBus>();
        	_pipeline = MessagePipelineConfigurator.CreateDefault(_builder, _bus);
        	var ef = MockRepository.GenerateStub<IEndpointResolver>();
            _address = new Uri("msmq://localhost/dru");
            _builder.Stub(x => x.GetInstance<IEndpointResolver>()).Return(ef);
            ef.Stub(x => x.GetEndpoint(_address)).Return(new NullEndpoint());

            _bus.Stub(x => x.OutboundPipeline).Return(_pipeline);
        }

        Uri _address;
        IServiceBus _bus;
        MessagePipeline _pipeline;
        IObjectBuilder _builder;

        [Test]
        public void ConfigurationTest()
        {
            var cfg = new RoutingConfigurator();

            cfg.Route<PingMessage>().To(_address);
            cfg.Route<PongMessage>().To(_address);


            IBusService c = cfg.Create(_bus, _builder);

            PipelineViewer.Trace(_pipeline);

            c.Start(_bus);

            PipelineViewer.Trace(_pipeline);

            c.Stop();

            PipelineViewer.Trace(_pipeline);
        }
    }
}