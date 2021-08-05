namespace MassTransit
{
    using System;


    public interface ITimeoutConfigurator
    {
        TimeSpan Timeout { set; }
    }
}
