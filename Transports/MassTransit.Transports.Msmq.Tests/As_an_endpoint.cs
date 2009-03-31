namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using Configuration;
    using Exceptions;
    using MassTransit.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class When_endpoint_doesnt_exist
    {
        [Test]
        [ExpectedException(typeof(EndpointException))]
        public void Should_throw_an_endpoint_exception()
        {
            new MsmqEndpoint("msmq://localhost/idontexist_tx");
        }

        [Test]
        [ExpectedException(typeof(EndpointException))]
        public void Should_throw_an_endpoint_exception_from_the_factory()
        {
            var ef = EndpointFactoryConfigurator.New(x =>
                {
                    x.RegisterTransport<MsmqEndpoint>();
                });
            ef.GetEndpoint("msmq://localhost/idontexist_tx");
        }
    }

    [TestFixture]
    public class When_instantiated_endpoints_via_uri
    {
        [Test]
        public void Should_get_conveted_into_a_fully_qualified_uri()
        {
            string endpointName = @"msmq://localhost/mt_client_tx";

            MsmqEndpoint defaultEndpoint = new MsmqEndpoint(endpointName);

            string machineEndpointName = endpointName.Replace("localhost", Environment.MachineName.ToLowerInvariant());

            defaultEndpoint.Uri.Equals(machineEndpointName)
                .ShouldBeTrue();
        }
    }
}