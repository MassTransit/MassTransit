namespace MassTransit.Persistence.Tests.ComponentTests.SqlServer
{
    using System.Data;
    using Common;
    using Integration.Saga;
    using NUnit.Framework;
    using Persistence.SqlServer.Connections;


    [TestFixture]
    public class SqlServer_PessimisticSqlBuilder_Tests : SqlServer_Tests
    {
        public class VersionedSaga_SqlBuilder
        {
            readonly PessimisticSqlServerDatabaseContext<VersionedSaga> _subject = new("", "VersionedSagas", "CorrelationId", IsolationLevel.Unspecified);

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = _subject.BuildInsertSql();
                var expected =
                    "INSERT INTO VersionedSagas ([CorrelationId], [RowVersion], [Name], [Age], [PhoneNumber], [Zip_Code]) VALUES (@correlationid, @rowversion, @name, @age, @phonenumber, @zipcode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = _subject.BuildUpdateSql();
                var expected =
                    "UPDATE VersionedSagas SET [RowVersion] = @rowversion, [Name] = @name, [Age] = @age, [PhoneNumber] = @phonenumber, [Zip_Code] = @zipcode WHERE [CorrelationId] = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = _subject.BuildDeleteSql();
                var expected = "DELETE FROM VersionedSagas WHERE [CorrelationId] = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = _subject.BuildLoadSql();
                var expected = "SELECT TOP 1 * FROM VersionedSagas WITH (UPDLOCK, ROWLOCK) WHERE [CorrelationId] = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Query_builds_correct_sql()
            {
                var actual = _subject.BuildQuerySql(x => x.Name == "test", null);
                var expected = "SELECT * FROM VersionedSagas WITH (UPDLOCK, ROWLOCK) WHERE [Name] = @value0";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }


        public class UnversionedSaga_SqlBuilder
        {
            protected SagaDatabaseContext<UnversionedSaga> Subject =
                new PessimisticSqlServerDatabaseContext<UnversionedSaga>("", "UnversionedSagas", "CorrelationId", IsolationLevel.Unspecified);

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = Subject.BuildInsertSql();
                var expected =
                    "INSERT INTO UnversionedSagas ([CorrelationId], [Name], [EarthTrips], [PhoneNumber], [Zip_Code]) VALUES (@correlationid, @name, @age, @phonenumber, @zipcode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = Subject.BuildUpdateSql();
                var expected =
                    "UPDATE UnversionedSagas SET [Name] = @name, [EarthTrips] = @age, [PhoneNumber] = @phonenumber, [Zip_Code] = @zipcode WHERE [CorrelationId] = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = Subject.BuildDeleteSql();
                var expected = "DELETE FROM UnversionedSagas WHERE [CorrelationId] = @correlationid";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = Subject.BuildLoadSql();
                var expected = "SELECT TOP 1 * FROM UnversionedSagas WITH (UPDLOCK, ROWLOCK) WHERE [CorrelationId] = @correlationid";

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
    }
}
