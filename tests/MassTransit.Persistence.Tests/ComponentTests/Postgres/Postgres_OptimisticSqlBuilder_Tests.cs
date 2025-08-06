namespace MassTransit.Persistence.Tests.ComponentTests.Postgres
{
    using Integration.Saga;
    using NUnit.Framework;
    using PostgreSql.Connections;


    [TestFixture]
    public class Postgres_OptimisticSqlBuilder_Tests : Postgres_Tests
    {
        public class VersionedSaga_SqlBuilder
        {
            readonly OptimisticPostgresDatabaseContext<VersionedSaga> _subject = new("", "VersionedSagas", "CorrelationId", "XMin");

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = _subject.BuildInsertSql();
                var expected = "INSERT INTO VersionedSagas (CorrelationId, Name, Age, PhoneNumber, Zip_Code) VALUES (@correlationid, @name, @age, @phonenumber, @zipcode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = _subject.BuildUpdateSql();
                var expected = "UPDATE VersionedSagas SET Name = @name, Age = @age, PhoneNumber = @phonenumber, Zip_Code = @zipcode WHERE CorrelationId = @correlationid AND xmin = @xmin";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = _subject.BuildDeleteSql();
                var expected = "DELETE FROM VersionedSagas WHERE CorrelationId = @correlationid AND xmin = @xmin";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = _subject.BuildLoadSql();
                var expected = "SELECT *, xmin AS XMin FROM VersionedSagas WHERE CorrelationId = @correlationid LIMIT 1";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Query_builds_correct_sql()
            {
                var actual = _subject.BuildQuerySql(x => x.Name == "test", null);
                var expected = "SELECT *, xmin AS XMin FROM VersionedSagas WHERE Name = @name";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}
