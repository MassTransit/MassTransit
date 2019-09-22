namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Contracts;
    using Internals.Reflection;


    public class ObjectInfoContractCache :
        IObjectInfoContractCache
    {
        readonly ConcurrentDictionary<string, Contract> _contractTypes;

        ObjectInfoContractCache()
        {
            _contractTypes = new ConcurrentDictionary<string, Contract>();
        }

        public static Contract GetOrAddContract(ObjectInfo objectInfo)
        {
            return Cached.Instance.Value.GetOrAddObjectInfo(objectInfo);
        }

        public static void AddContracts(params ObjectInfo[] objectInfos)
        {
            Cached.Instance.Value.AddContracts(objectInfos);
        }

        Contract IObjectInfoContractCache.GetOrAddObjectInfo(ObjectInfo objectInfo)
        {
            return _contractTypes.GetOrAdd(objectInfo.ObjectType, x => CreateContract(objectInfo));
        }

        void IObjectInfoContractCache.AddContracts(params ObjectInfo[] objectInfos)
        {
            foreach (var objectInfo in objectInfos)
            {
                _contractTypes.AddOrUpdate(objectInfo.ObjectType, CreateContract(objectInfo), (_, existing) => existing);
            }
        }

        Contract CreateContract(ObjectInfo objectInfo)
        {
            List<Property> properties = new List<Property>();

            foreach (var propertyInfo in objectInfo.Properties)
            {
                if ((propertyInfo.Kind & PropertyKind.CollectionMask) == PropertyKind.Dictionary)
                {
                    var keyType = GetPropertyType(propertyInfo.KeyType, PropertyKind.Value);
                    var valueType = GetPropertyType(propertyInfo.PropertyType, propertyInfo.Kind);

                    var propertyType = typeof(IDictionary<,>).MakeGenericType(keyType, valueType);

                    properties.Add(new Property(propertyInfo.Name, propertyType));
                }
                else if ((propertyInfo.Kind & PropertyKind.CollectionMask) == PropertyKind.Array)
                {
                    var valueType = GetPropertyType(propertyInfo.PropertyType, propertyInfo.Kind);

                    var propertyType = typeof(IList<>).MakeGenericType(valueType);

                    properties.Add(new Property(propertyInfo.Name, propertyType));
                }
                else
                {
                    var propertyType = GetPropertyType(propertyInfo.PropertyType, propertyInfo.Kind);

                    properties.Add(new Property(propertyInfo.Name, propertyType));
                }
            }

            return new Contract(objectInfo.ObjectType, properties.ToArray());
        }

        static Type GetPropertyType(string propertyType, PropertyKind kind)
        {
            if (kind.HasFlag(PropertyKind.Object))
            {
                if (ObjectInfoCache.TryGetObjectInfo(propertyType, out var propertyObjectInfo))
                {
                    var contract = GetOrAddContract(propertyObjectInfo);

                    return ContractCache.GetContractType(contract);
                }
            }
            else
            {
                var type = Type.GetType(propertyType, true);

                if (kind.HasFlag(PropertyKind.Nullable))
                    type = typeof(Nullable<>).MakeGenericType(type);

                return type;
            }

            throw new ArgumentException($"Unable to determine type for property: {propertyType}", nameof(propertyType));
        }


        static class Cached
        {
            internal static readonly Lazy<IObjectInfoContractCache> Instance = new Lazy<IObjectInfoContractCache>(() => new ObjectInfoContractCache());
        }
    }
}
