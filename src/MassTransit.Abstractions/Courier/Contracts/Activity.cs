namespace MassTransit.Courier.Contracts
{
    using System;
    using System.Collections.Generic;


    public interface Activity
    {
        string Name { get; }

        Uri Address { get; }

        IDictionary<string, object> Arguments { get; }
    }
}
