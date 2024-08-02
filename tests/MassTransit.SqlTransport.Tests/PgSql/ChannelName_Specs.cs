namespace MassTransit.DbTransport.Tests.PgSql;

using System.Threading.Tasks;
using NUnit.Framework;
using SqlTransport.PostgreSql.Helpers;

[TestFixture]
public class ChannelName_Specs
{
    [TestCase("string that has 40 characters 1234567890")]
    public async Task Should_sanitize_schema(string schema)
    {
        var sanitizedSchema = NotifyChannel.SanitizeSchemaName(schema);

        Assert.That(sanitizedSchema, Has.Length.EqualTo(39));
        Assert.That(sanitizedSchema, Is.EqualTo("string that has 40 characters 123456789"));
    }

    [TestCase("string that has 38 characters 12345678")]
    [TestCase("string that has 39 characters 123456789")]
    public async Task Should_not_sanitize_schema(string schema)
    {
        var sanitizedSchema = NotifyChannel.SanitizeSchemaName(schema);

        Assert.That(sanitizedSchema, Is.EqualTo(schema));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public async Task Should_return_default(string schema)
    {
        var sanitizedSchema = NotifyChannel.SanitizeSchemaName(schema);

        Assert.That(sanitizedSchema, Is.EqualTo("transport"));
    }
}
