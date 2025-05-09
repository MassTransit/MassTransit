namespace MassTransit.DapperIntegration.Tests.ComponentTests
{
    using System;
    using Common;
    using MassTransit.DapperIntegration.Tests.IntegrationTests.ConsumerSagas;
    using MassTransit.DapperIntegration.Tests.IntegrationTests.StateMachineSagas;
    using NUnit.Framework;
    using SqlBuilders;


    [TestFixture]
    public class SqlServer_SqlBuilder_Tests
    {
        public class VersionedSaga_SqlBuilder
        {
            protected ISagaSqlFormatter<VersionedSaga> Subject = new SqlServerSagaFormatter<VersionedSaga>();
            
            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = Subject.BuildInsertSql();
                var expected = "INSERT INTO VersionedSagas ([CorrelationId], [Version], [Name], [Age], [PhoneNumber], [Zip_Code]) VALUES (@correlationId, @version, @name, @age, @phoneNumber, @zipCode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = Subject.BuildUpdateSql();
                var expected = "UPDATE VersionedSagas SET [Name] = @name, [Age] = @age, [PhoneNumber] = @phoneNumber, [Zip_Code] = @zipCode, [Version] = @version WHERE [CorrelationId] = @correlationId AND [Version] < @version";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = Subject.BuildDeleteSql();
                var expected = "DELETE FROM VersionedSagas WHERE [CorrelationId] = @correlationId AND [Version] < @version";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = Subject.BuildLoadSql();
                var expected = "SELECT * FROM VersionedSagas WITH (UPDLOCK, ROWLOCK) WHERE [CorrelationId] = @correlationId";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Query_builds_correct_sql()
            {
                var actual = Subject.BuildQuerySql(x => x.Name == "test", null);
                var expected = "SELECT * FROM VersionedSagas WITH (UPDLOCK, ROWLOCK) WHERE [Name] = @value0";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        public class UnversionedSaga_SqlBuilder
        {
            protected ISagaSqlFormatter<UnversionedSaga> Subject = new SqlServerSagaFormatter<UnversionedSaga>();

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = Subject.BuildInsertSql();
                var expected = "INSERT INTO UnversionedSagas ([CorrelationId], [Name], [EarthTrips], [PhoneNumber], [Zip_Code]) VALUES (@correlationId, @name, @age, @phoneNumber, @zipCode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = Subject.BuildUpdateSql();
                var expected = "UPDATE UnversionedSagas SET [Name] = @name, [EarthTrips] = @age, [PhoneNumber] = @phoneNumber, [Zip_Code] = @zipCode WHERE [CorrelationId] = @correlationId";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = Subject.BuildDeleteSql();
                var expected = "DELETE FROM UnversionedSagas WHERE [CorrelationId] = @correlationId";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = Subject.BuildLoadSql();
                var expected = "SELECT * FROM UnversionedSagas WITH (UPDLOCK, ROWLOCK) WHERE [CorrelationId] = @correlationId";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Query_builds_correct_sql()
            {
                var actual = Subject.BuildQuerySql(x => x.Name == "test" && x.Age < 99, null);
                var expected = "SELECT * FROM UnversionedSagas WITH (UPDLOCK, ROWLOCK) WHERE [Name] = @value0 AND [EarthTrips] < @value1";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
        
        public class Complex_SqlBuilder
        {
            protected ISagaSqlFormatter<ComplexSaga> Subject = new SqlServerSagaFormatter<ComplexSaga>();


            [Test]
            public void ComplexExpressions_behave_properly()
            {
                var m = new { Start = new DateTime(2025, 04, 22), End = new DateTime(2025, 05, 22) };
                var actual = Subject.BuildQuerySql(x => x.Name == "test" && x.Age <= 99 && x.StartDate > m.Start && x.EndDate < m.End, null);
                var expected = "SELECT * FROM OverrideTable WITH (UPDLOCK, ROWLOCK) WHERE [Name] = @value0 AND [Age] <= @value1 AND [StartDate] > @value2 AND [EndDate] < @value3";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        public class Prefixed_SqlBuilder
        {
            protected ISagaSqlFormatter<PrefixedSaga> Subject;

            [SetUp]
            public void Prepare()
            {
                Subject = new SqlServerSagaFormatter<PrefixedSaga>();
            }

            [Test]
            public void Prefix_mapping_applies_prefix()
            {
                Subject.MapPrefix(m => m.Nested);

                var actual = Subject.BuildQuerySql(x => x.Nested.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WITH (UPDLOCK, ROWLOCK) WHERE [NestedId] = @value0";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Prefix_mapping_is_configurable()
            {
                Subject.MapPrefix(m => m.Nested, "nst_");

                var actual = Subject.BuildQuerySql(x => x.Nested.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WITH (UPDLOCK, ROWLOCK) WHERE [nst_Id] = @value0";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Prefix_mapping_prefix_is_specific()
            {
                Subject.MapPrefix(m => m.Nested);

                var actual = Subject.BuildQuerySql(x => x.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WITH (UPDLOCK, ROWLOCK) WHERE [Id] = @value0";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Property_mapping_is_exact()
            {
                Subject.MapProperty(m => m.Nested.Id, "MyId");

                var actual = Subject.BuildQuerySql(x => x.Nested.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WITH (UPDLOCK, ROWLOCK) WHERE [MyId] = @value0";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Property_mapping_is_specific()
            {
                Subject.MapProperty(m => m.Nested.Id, "MyId");

                var actual = Subject.BuildQuerySql(x => x.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WITH (UPDLOCK, ROWLOCK) WHERE [Id] = @value0";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Property_prefix_does_not_conflict()
            {
                Subject.MapPrefix(m => m.Nested);
                Subject.MapPrefix(m => m.Optional);

                var actual = Subject.BuildQuerySql(x => x.Id == 10 && x.Nested.Id == 11 && x.Optional.Id == 12, null);
                var expected = "SELECT * FROM PrefixedSagas WITH (UPDLOCK, ROWLOCK) WHERE [Id] = @value0 AND [NestedId] = @value1 AND [OptionalId] = @value2";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Property_mapping_does_not_conflict()
            {
                Subject.MapProperty(m => m.Optional.Id, "id2");
                Subject.MapProperty(m => m.Nested.Id, "id1");
                
                var actual = Subject.BuildQuerySql(x => x.Nested.Id == 11 && x.Optional.Id == 12, null);
                var expected = "SELECT * FROM PrefixedSagas WITH (UPDLOCK, ROWLOCK) WHERE [id1] = @value0 AND [id2] = @value1";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}
