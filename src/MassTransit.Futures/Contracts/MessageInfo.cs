namespace MassTransit.Contracts
{
    using System;


    public interface MessageInfo
    {
        string Name { get; }

        Uri MessageType { get; }

        PropertyInfo[] Properties { get; }
    }
}