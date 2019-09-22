namespace MassTransit.Internals.Reflection
{
    using System;


    public interface IContractTypeBuilder
    {
        Type GetContractType(Contract contract);
    }
}
