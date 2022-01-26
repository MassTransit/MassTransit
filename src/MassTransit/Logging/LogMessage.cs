#nullable enable
namespace MassTransit.Logging
{
    using System;


    public delegate void LogMessage<in T1>(T1 arg1, Exception? exception = default);


    public delegate void LogMessage<in T1, in T2>(T1 arg1, T2 arg2, Exception? exception = default);


    public delegate void LogMessage<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3, Exception? exception = default);


    public delegate void LogMessage<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? exception = default);


    public delegate void LogMessage<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? exception = default);
}
