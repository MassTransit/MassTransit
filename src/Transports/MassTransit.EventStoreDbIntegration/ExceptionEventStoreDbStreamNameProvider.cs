using System;
using MassTransit.EventStoreDbIntegration;

namespace Automatonymous
{
    /// <summary>
    /// Return the name of the EventStoreDB stream
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate StreamName ExceptionEventStoreDbStreamNameProvider<in TInstance, in TData, in TException>(ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TException : Exception
        where TData : class;


    /// <summary>
    /// Return the name of the EventStoreDB stream
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate StreamName ExceptionEventStoreDbStreamNameProvider<in TInstance, in TException>(ConsumeExceptionEventContext<TInstance, TException> context)
        where TException : Exception;
}
