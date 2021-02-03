namespace MassTransit.Pipeline
{
    using System;


    [Flags]
    public enum ConnectPipeOptions
    {
        ConfigureConsumeTopology = 1,

        All = ConfigureConsumeTopology
    }
}
