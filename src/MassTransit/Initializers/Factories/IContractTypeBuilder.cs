namespace MassTransit.Initializers.Factories
{
    using System;


    public interface IContractTypeBuilder
    {
        Type GetContractType(Contract contract);
    }
}
