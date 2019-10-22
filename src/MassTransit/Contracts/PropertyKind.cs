namespace MassTransit.Contracts
{
    using System;


    [Flags]
    public enum PropertyKind
    {
        Value = 0,
        Object = 1,
        Array = 2,
        Dictionary = 4,
        Nullable = 8,

        CollectionMask = 0x6
    }
}
