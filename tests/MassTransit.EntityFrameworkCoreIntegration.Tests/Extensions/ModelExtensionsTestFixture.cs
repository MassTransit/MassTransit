namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Extensions
{
    using ContainerTests;
    using EntityFrameworkCoreIntegration.Extensions;
    using NUnit.Framework;
    using Shared;
    using Shouldly;

    [TestFixture(typeof(SqlServerTestDbParameters))]
    public class Getting_an_entity_type_via_extension<T> : EntityFrameworkTestFixture<T, TestInstanceDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public void Should_return_the_expected_type()
        {
            using var db = new TestInstanceContextFactory().CreateDbContext(DbContextOptionsBuilder);
            var entityType = db.Model.SafeFindEntityType(typeof(TestInstance));

            _ = entityType.ShouldNotBeNull();
            entityType.ClrType.ShouldBe(typeof(TestInstance));
        }
    }
}
