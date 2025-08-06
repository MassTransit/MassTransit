namespace MassTransit.Persistence.Tests.ComponentTests.Postgres
{
    using System.Data;
    using Common;
    using Integration.Saga;
    using NUnit.Framework;
    using PostgreSql.Connections;


    [TestFixture]
    public class Postgres_PessimisticSqlBuilder_Tests : Postgres_Tests
    {
        public class VersionedSaga_SqlBuilder
        {
            readonly PessimisticPostgresDatabaseContext<VersionedSaga> _subject = new("", "VersionedSagas", "CorrelationId", IsolationLevel.Unspecified);

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = _subject.BuildInsertSql();
                var expected =
                    "INSERT INTO VersionedSagas (CorrelationId, XMin, Name, Age, PhoneNumber, Zip_Code) VALUES (@correlationid, @xmin, @name, @age, @phonenumber, @zipcode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = _subject.BuildUpdateSql();
                var expected =
                    "UPDATE VersionedSagas SET XMin = @xmin, Name = @name, Age = @age, PhoneNumber = @phonenumber, Zip_Code = @zipcode WHERE CorrelationId = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = _subject.BuildDeleteSql();
                var expected = "DELETE FROM VersionedSagas WHERE CorrelationId = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = _subject.BuildLoadSql();
                var expected = "SELECT * FROM VersionedSagas WHERE CorrelationId = @correlationid FOR UPDATE LIMIT 1";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Query_builds_correct_sql()
            {
                var actual = _subject.BuildQuerySql(x => x.Name == "test", null);
                var expected = "SELECT * FROM VersionedSagas WHERE Name = @name FOR UPDATE";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }


        public class UnversionedSaga_SqlBuilder
        {
            protected SagaDatabaseContext<UnversionedSaga> Subject =
                new PessimisticPostgresDatabaseContext<UnversionedSaga>("", "UnversionedSagas", "CorrelationId", IsolationLevel.Unspecified);

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = Subject.BuildInsertSql();
                var expected =
                    "INSERT INTO UnversionedSagas (CorrelationId, Name, EarthTrips, PhoneNumber, Zip_Code) VALUES (@correlationid, @name, @age, @phonenumber, @zipcode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = Subject.BuildUpdateSql();
                var expected =
                    "UPDATE UnversionedSagas SET Name = @name, EarthTrips = @age, PhoneNumber = @phonenumber, Zip_Code = @zipcode WHERE CorrelationId = @correlationid";

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
                var expected = "SELECT * FROM UnversionedSagas WHERE CorrelationId = @correlationid FOR UPDATE LIMIT 1";

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
    }
}
