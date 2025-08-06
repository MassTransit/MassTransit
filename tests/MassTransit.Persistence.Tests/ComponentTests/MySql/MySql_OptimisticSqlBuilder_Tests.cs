namespace MassTransit.Persistence.Tests.ComponentTests.MySql
{
    using Integration.Saga;
    using NUnit.Framework;
    using Persistence.MySql.Connections;


    [TestFixture]
    public class MySql_OptimisticSqlBuilder_Tests : MySql_Tests
    {
        public class VersionedSaga_SqlBuilder
        {
            readonly OptimisticMySqlDatabaseContext<VersionedSaga> _subject = new("", "VersionedSagas", "CorrelationId", "RowVersion", "RowVersion");

            [Test]
            public void Insert_builds_correct_sql()
            {
                var actual = _subject.BuildInsertSql();
                var expected =
                    "INSERT INTO VersionedSagas (CorrelationId, Name, Age, PhoneNumber, Zip_Code) VALUES (@correlationid, @name, @age, @phonenumber, @zipcode)";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Update_builds_correct_sql()
            {
                var actual = _subject.BuildUpdateSql();
                var expected =
                    "UPDATE VersionedSagas SET Name = @name, Age = @age, PhoneNumber = @phonenumber, Zip_Code = @zipcode WHERE CorrelationId = @correlationid AND RowVersion = @rowversion";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Delete_builds_correct_sql()
            {
                var actual = _subject.BuildDeleteSql();
                var expected = "DELETE FROM VersionedSagas WHERE CorrelationId = @correlationid AND RowVersion = @rowversion";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Load_builds_correct_sql()
            {
                var actual = _subject.BuildLoadSql();
                var expected = "SELECT * FROM VersionedSagas WHERE CorrelationId = @correlationid LIMIT 1";

                Assert.That(actual, Is.EqualTo(expected));
            }

            [Test]
            public void Query_builds_correct_sql()
            {
                var actual = _subject.BuildQuerySql(x => x.Name == "test", null);
                var expected = "SELECT * FROM VersionedSagas WHERE Name = @name";

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}
