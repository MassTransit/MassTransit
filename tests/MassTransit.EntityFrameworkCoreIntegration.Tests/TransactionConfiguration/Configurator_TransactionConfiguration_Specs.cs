namespace MassTransit.EntityFrameworkCoreIntegration.Tests.TransactionConfiguration
{
    using System;
    using System.Data;
    using MassTransit.Configuration;
    using MassTransit.Tests.Saga;
    using NUnit.Framework;


    /// <summary>
    /// Tests for EntityFrameworkSagaRepositoryConfigurator transaction configuration functionality
    /// </summary>
    [TestFixture]
    public class Configurator_TransactionConfiguration_Specs
    {
        [Test]
        public void Should_accept_configure_transaction_false()
        {
            var configurator = new EntityFrameworkSagaRepositoryConfigurator<SimpleSaga>();
            
            Assert.DoesNotThrow(() => configurator.ConfigureTransaction(false),
                "ConfigureTransaction(false) should not throw");
        }

        [Test]
        public void Should_accept_configure_transaction_true()
        {
            var configurator = new EntityFrameworkSagaRepositoryConfigurator<SimpleSaga>();
            
            Assert.DoesNotThrow(() => configurator.ConfigureTransaction(true),
                "ConfigureTransaction(true) should not throw");
        }

        [Test]
        public void Should_allow_multiple_configuration_calls()
        {
            var configurator = new EntityFrameworkSagaRepositoryConfigurator<SimpleSaga>();
            
            Assert.DoesNotThrow(() => {
                configurator.ConfigureTransaction(false);
                configurator.ConfigureTransaction(true);
                configurator.ConfigureTransaction(false);
            }, "Multiple ConfigureTransaction calls should not throw");
        }

        [Test]
        public void Should_support_fluent_configuration()
        {
            var configurator = new EntityFrameworkSagaRepositoryConfigurator<SimpleSaga>();
            
            Assert.DoesNotThrow(() => {
                configurator.ConcurrencyMode = ConcurrencyMode.Optimistic;
                configurator.ConfigureTransaction(false);
                configurator.IsolationLevel = IsolationLevel.ReadCommitted;
            }, "Fluent configuration with transaction setting should work");
        }
    }
}