namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Net.Security;
    using System.Security.Authentication;
    using NUnit.Framework;
    using RabbitMqTransport.Configuration;
    using Shouldly;
    using Shouldly.ShouldlyExtensionMethods;


    [TestFixture]
    public class HostConfigurator_Specs
    {
        [Test]
        [Description("Default SSL settings should allow remote certificate chain error to maintain backward compatibility.")]
        public void Should_allow_remote_certificate_chain_error_by_default()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator =>
            {
            });

            configurator.Settings.AcceptablePolicyErrors.ShouldHaveFlag(SslPolicyErrors.RemoteCertificateChainErrors);
        }

        [Test]
        [Description("Remote certificate chain errors should be enforced when set.")]
        public void Should_enforce_remote_certificate_chain_error_when_set()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator =>
            {
                sslConfigurator.EnforcePolicyErrors(SslPolicyErrors.RemoteCertificateChainErrors);
            });

            configurator.Settings.AcceptablePolicyErrors.ShouldNotHaveFlag(SslPolicyErrors.RemoteCertificateChainErrors);
        }

        [Test]
        [Description("Should parse vhost with escape characteres %2f")]
        public void Should_ParseVhost_With_escapes()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost/%2fv%2fhost"));

            configurator.Settings.VirtualHost.ShouldBe("/v/host");
        }

        [Test]
        public void Should_set_assigned_ssl_protocol()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator =>
            {
                sslConfigurator.Protocol = SslProtocols.Tls12;
            });

            configurator.Settings.SslProtocol.ShouldBe(SslProtocols.Tls12);
        }

        [Test]
        [Description("Default SSL settings should not use client certificate as authentication identity")]
        public void Should_set_client_certificate_as_authentication_identity_as_false_by_default()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator =>
            {
            });

            configurator.Settings.UseClientCertificateAsAuthenticationIdentity.ShouldBeFalse();
        }

        [Test]
        [Description("Default ssl protocol should be tls 1.0 for back compatibility reason")]
        public void Should_set_tls10_protocol_by_default()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator =>
            {
            });

            configurator.Settings.SslProtocol.ShouldBe(SslProtocols.Tls);
        }

        [Test]
        [Description("Custom client certificate selector should be used when set.")]
        public void Should_use_custom_client_certificate_selector_when_set()
        {
            LocalCertificateSelectionCallback customSelector =
                (sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers)
                    => null;

            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));
            configurator.UseSsl(sslConfigurator =>
            {
                sslConfigurator.CertificateSelectionCallback = customSelector;
            });

            configurator.Settings.CertificateSelectionCallback.ShouldBeSameAs(customSelector);
        }

        [Test]
        [Description("Custom remote certificate validator should be used when set.")]
        public void Should_use_custom_remote_certificate_validator_when_set()
        {
            RemoteCertificateValidationCallback customValidator =
                (sender, certificate, chain, sslPolicyErrors)
                    => false;

            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));
            configurator.UseSsl(sslConfigurator =>
            {
                sslConfigurator.CertificateValidationCallback = customValidator;
            });

            configurator.Settings.CertificateValidationCallback.ShouldBeSameAs(customValidator);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Description("SSL settings should set use client certificate as authentication identity as specified by configuration")]
        public void Should_set_client_certificate_as_authentication_identity_when_configured(bool valueToSet)
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator =>
            {
                sslConfigurator.UseCertificateAsAuthenticationIdentity = valueToSet;
            });

            configurator.Settings.UseClientCertificateAsAuthenticationIdentity.ShouldBe(valueToSet);
        }
    }
}
