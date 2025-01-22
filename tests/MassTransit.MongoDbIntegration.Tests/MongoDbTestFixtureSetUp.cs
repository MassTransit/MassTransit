namespace MassTransit.MongoDbIntegration.Tests;

using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NUnit.Framework;


[SetUpFixture]
public class MongoDbTestFixtureSetUp
{
    [OneTimeSetUp]
    public async Task Before_any()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }
}
