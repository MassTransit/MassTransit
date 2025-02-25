namespace MassTransit.Configuration;

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;


static class MongoDbGuidRepresentationExtensions
{
    static readonly IBsonSerializer<Guid> _guidSerializer = new GuidSerializer(GuidRepresentation.Standard);
    static readonly IBsonSerializer<Guid?> _nullableGuidSerializer = new NullableSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard));

    internal static BsonMemberMap EnsureGuidRepresentationSpecified(this BsonMemberMap memberMap)
    {
        if (memberMap.MemberType != typeof(Guid) && memberMap.MemberType != typeof(Guid?))
            throw new ArgumentException("Only Guid and nullable guids are supported", nameof(memberMap));

        IBsonSerializer<Guid> bsonSerializer = BsonSerializer.SerializerRegistry.GetSerializer<Guid>();

        return bsonSerializer is null or GuidSerializer { GuidRepresentation: GuidRepresentation.Unspecified }
            ? memberMap.SetSerializer(memberMap.MemberType == typeof(Guid) ? _guidSerializer : _nullableGuidSerializer)
            : memberMap;
    }
}
