namespace MassTransit.Internals.Reflection
{
    using System;


    public interface IImplementationBuilder
    {
        Type GetImplementationType(Type interfaceType);
    }
}