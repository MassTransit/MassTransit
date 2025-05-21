namespace MassTransit.DapperIntegration.Tests.ComponentTests
{
    using System;
    using Common;
    using NUnit.Framework;
    using SqlBuilders;


    [TestFixture]
    public class Postgres_SqlBuilder_Tests
    {
        public class VersionedSaga_SqlBuilder
        {
            protected ISagaSqlFormatter<VersionedSaga> Subject = new PostgresSagaFormatter<VersionedSaga>();

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = Subject.BuildInsertSql();
                var expected = "INSERT INTO VersionedSagas (CorrelationId, Version, Name, Age, PhoneNumber, Zip_Code) VALUES (@correlationid, @version, @name, @age, @phonenumber, @zipcode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = Subject.BuildUpdateSql();
                var expected = "UPDATE VersionedSagas SET Version = @version, Name = @name, Age = @age, PhoneNumber = @phonenumber, Zip_Code = @zipcode WHERE CorrelationId = @correlationid AND Version < @version";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = Subject.BuildDeleteSql();
                var expected = "DELETE FROM VersionedSagas WHERE CorrelationId = @correlationid AND Version < @version";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = Subject.BuildLoadSql();
                var expected = "SELECT * FROM VersionedSagas WHERE CorrelationId = @correlationid FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Query_builds_correct_sql()
            {
                var actual = Subject.BuildQuerySql(x => x.Name == "test", null);
                var expected = "SELECT * FROM VersionedSagas WHERE Name = @name FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        public class UnversionedSaga_SqlBuilder
        {
            protected ISagaSqlFormatter<UnversionedSaga> Subject = new PostgresSagaFormatter<UnversionedSaga>();

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = Subject.BuildInsertSql();
                var expected = "INSERT INTO UnversionedSagas (CorrelationId, Name, EarthTrips, PhoneNumber, Zip_Code) VALUES (@correlationid, @name, @age, @phonenumber, @zipcode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = Subject.BuildUpdateSql();
                var expected = "UPDATE UnversionedSagas SET Name = @name, EarthTrips = @age, PhoneNumber = @phonenumber, Zip_Code = @zipcode WHERE CorrelationId = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = Subject.BuildDeleteSql();
                var expected = "DELETE FROM UnversionedSagas WHERE CorrelationId = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = Subject.BuildLoadSql();
                var expected = "SELECT * FROM UnversionedSagas WHERE CorrelationId = @correlationid FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Query_builds_correct_sql()
            {
                var actual = Subject.BuildQuerySql(x => x.Name == "test" && x.Age < 99, null);
                var expected = "SELECT * FROM UnversionedSagas WHERE Name = @name AND EarthTrips < @earthtrips FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        public class Complex_SqlBuilder
        {
            protected ISagaSqlFormatter<ComplexSaga> Subject = new PostgresSagaFormatter<ComplexSaga>();


            [Test]
            public void ComplexExpressions_behave_properly()
            {
                var m = new { Start = new DateTime(2025, 04, 22), End = new DateTime(2025, 05, 22) };
                var actual = Subject.BuildQuerySql(x => x.Name == "test" && x.Age <= 99 && x.StartDate > m.Start && x.EndDate < m.End, null);
                var expected = "SELECT * FROM OverrideTable WHERE Name = @name AND Age <= @age AND StartDate > @startdate AND EndDate < @enddate FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        public class Prefixed_SqlBuilder
        {
            protected ISagaSqlFormatter<PrefixedSaga> Subject;

            [SetUp]
            public void Prepare()
            {
                Subject = new PostgresSagaFormatter<PrefixedSaga>();
            }

            [Test]
            public void Prefix_mapping_applies_prefix()
            {
                Subject.MapPrefix(m => m.Nested);

                var actual = Subject.BuildQuerySql(x => x.Nested.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WHERE NestedId = @nestedid FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Prefix_mapping_is_configurable()
            {
                Subject.MapPrefix(m => m.Nested, "nst_");

                var actual = Subject.BuildQuerySql(x => x.Nested.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WHERE nst_Id = @nst_id FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Prefix_mapping_prefix_is_specific()
            {
                Subject.MapPrefix(m => m.Nested);

                var actual = Subject.BuildQuerySql(x => x.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WHERE Id = @id FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Property_mapping_is_exact()
            {
                Subject.MapProperty(m => m.Nested.Id, "MyId");

                var actual = Subject.BuildQuerySql(x => x.Nested.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WHERE MyId = @myid FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Property_mapping_is_specific()
            {
                Subject.MapProperty(m => m.Nested.Id, "MyId");

                var actual = Subject.BuildQuerySql(x => x.Id == 10, null);
                var expected = "SELECT * FROM PrefixedSagas WHERE Id = @id FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Property_prefix_does_not_conflict()
            {
                Subject.MapPrefix(m => m.Nested);
                Subject.MapPrefix(m => m.Optional);

                var actual = Subject.BuildQuerySql(x => x.Id == 10 && x.Nested.Id == 11 && x.Optional.Id == 12, null);
                var expected = "SELECT * FROM PrefixedSagas WHERE Id = @id AND NestedId = @nestedid AND OptionalId = @optionalid FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Property_mapping_does_not_conflict()
            {
                Subject.MapProperty(m => m.Optional.Id, "id2");
                Subject.MapProperty(m => m.Nested.Id, "id1");

                var actual = Subject.BuildQuerySql(x => x.Nested.Id == 11 && x.Optional.Id == 12, null);
                var expected = "SELECT * FROM PrefixedSagas WHERE id1 = @id1 AND id2 = @id2 FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}
