namespace MassTransit.Configuration;

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;


static class MongoDbGuidRepresentationExtensions
{
    static readonly IBsonSerializer<Guid> _guidSerializer;
    static readonly IBsonSerializer<Guid?> _nullableGuidSerializer;
    static readonly IBsonSerializer<List<Guid>> _listGuidSerializer;
    static readonly IBsonSerializer<List<Guid?>> _listNullableGuidSerializer;

    static MongoDbGuidRepresentationExtensions()
    {
        _guidSerializer = new GuidSerializer(GuidRepresentation.Standard);
        _nullableGuidSerializer = new NullableSerializer<Guid>(_guidSerializer);
        _listGuidSerializer = new EnumerableInterfaceImplementerSerializer<List<Guid>>(_guidSerializer);
        _listNullableGuidSerializer = new EnumerableInterfaceImplementerSerializer<List<Guid?>>(_nullableGuidSerializer);
    }

    internal static BsonMemberMap EnsureGuidRepresentationSpecified(this BsonMemberMap memberMap)
    {
        if (memberMap.MemberType != typeof(Guid) && memberMap.MemberType != typeof(Guid?))
            throw new ArgumentException("Only Guid and nullable guids are supported", nameof(memberMap));

        IBsonSerializer<Guid> bsonSerializer = BsonSerializer.SerializerRegistry.GetSerializer<Guid>();

        return bsonSerializer is null or GuidSerializer { GuidRepresentation: GuidRepresentation.Unspecified }
            ? memberMap.SetSerializer(memberMap.MemberType == typeof(Guid) ? _guidSerializer : _nullableGuidSerializer)
            : memberMap;
    }

    internal static BsonMemberMap EnsureListGuidRepresentationSpecified(this BsonMemberMap memberMap)
    {
        if (memberMap.MemberType != typeof(List<Guid>) && memberMap.MemberType != typeof(List<Guid?>))
            throw new ArgumentException("Only Guid and nullable guids are supported", nameof(memberMap));

        if (memberMap.MemberType == typeof(List<Guid>))
        {
            IBsonSerializer<List<Guid>> listGuidSerializer = BsonSerializer.SerializerRegistry.GetSerializer<List<Guid>>();
            if (listGuidSerializer is null or EnumerableInterfaceImplementerSerializer<List<Guid>>
                {
                    ItemSerializer: GuidSerializer { GuidRepresentation: GuidRepresentation.Unspecified }
                })
            {
                IBsonSerializer<Guid> guidSerializer = BsonSerializer.SerializerRegistry.GetSerializer<Guid>();

                if (guidSerializer is null or GuidSerializer { GuidRepresentation: GuidRepresentation.Unspecified })
                    guidSerializer = _guidSerializer;

                listGuidSerializer = new EnumerableInterfaceImplementerSerializer<List<Guid>>(guidSerializer);

                return memberMap.SetSerializer(listGuidSerializer);
            }
        }
        else
        {
            IBsonSerializer<List<Guid?>> listGuidSerializer = BsonSerializer.SerializerRegistry.GetSerializer<List<Guid?>>();
            if (listGuidSerializer is null or EnumerableInterfaceImplementerSerializer<List<Guid?>>
                {
                    ItemSerializer: NullableSerializer<Guid>
                    {
                        ValueSerializer: GuidSerializer { GuidRepresentation: GuidRepresentation.Unspecified }
                    }
                })
            {
                IBsonSerializer<Guid?> guidSerializer = BsonSerializer.SerializerRegistry.GetSerializer<Guid?>();
                if (guidSerializer is null or NullableSerializer<Guid>
                    {
                        ValueSerializer: GuidSerializer { GuidRepresentation: GuidRepresentation.Unspecified }
                    })
                    guidSerializer = _nullableGuidSerializer;

                listGuidSerializer = new EnumerableInterfaceImplementerSerializer<List<Guid?>>(guidSerializer);

                return memberMap.SetSerializer(listGuidSerializer);
            }
        }

        return memberMap;
    }
}
