namespace MassTransit.Metadata
{
    using Contracts;
    using Internals.Reflection;


    public interface IObjectInfoContractCache
    {
        Contract GetOrAddObjectInfo(ObjectInfo objectInfo);

        void AddContracts(params ObjectInfo[] objectInfos);
    }
}
