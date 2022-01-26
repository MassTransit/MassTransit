#nullable enable
namespace MassTransit.Logging
{
    using System;


    class NullScope :
        IDisposable
    {
        NullScope()
        {
        }

        public static NullScope Instance { get; } = new NullScope();

        public void Dispose()
        {
        }
    }
}
