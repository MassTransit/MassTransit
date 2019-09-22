namespace MassTransit.Tests.Initializers
{
    using System.Linq;
    using System.Reflection;
    using Internals.Reflection;
    using NUnit.Framework;
    using TestContracts;


    namespace TestContracts
    {
        public interface SimpleArgument
        {
            string Name { get; }
            int Age { get; }
        }
    }



    [TestFixture]
    public class Extracting_contract_from_type
    {
        [Test]
        public void Should_handle_simple_arguments()
        {
            var contracts = ContractCache<SimpleArgument>.Contracts;

            Assert.That(contracts, Is.Not.Null);
            Assert.That(contracts.Length, Is.EqualTo(1));

            var contract = contracts[0];

            Assert.That(contract.Properties, Is.Not.Null);
            Assert.That(contract.Properties.Length, Is.EqualTo(2));
        }

        [Test]
        public void Should_create_a_dynamic_type()
        {
            var contract = ContractCache<SimpleArgument>.Contracts.First();

            var messageFactory = ContractCache.GetMessageFactory(contract);

            var message = messageFactory.Create();

            Assert.That(message, Is.Not.Null);

            var messageType = message.GetType();

            TestContext.WriteLine(messageType.FullName);
            TestContext.WriteLine(messageType.Assembly.FullName);


            PropertyInfo[] properties = messageType.GetProperties();
            Assert.That(properties.Length, Is.EqualTo(2));
        }
    }
}
