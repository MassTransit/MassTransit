namespace MassTransit.Metadata
{
    using Contracts.Metadata;
    using Internals.Reflection;


    public interface IObjectInfoContractCache
    {
        Contract GetOrAddObjectInfo(ObjectInfo objectInfo);

        void AddContracts(params ObjectInfo[] objectInfos);
    }
}
