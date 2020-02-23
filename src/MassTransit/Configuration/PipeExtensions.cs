namespace MassTransit
{
    using System.Runtime.CompilerServices;
    using GreenPipes;
    using GreenPipes.Pipes;


    public static class PipeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty<T>(this IPipe<T> pipe)
            where T : class, PipeContext
        {
            if (pipe == null)
                return false;

            if (pipe is EmptyPipe<T>)
                return false;

            return true;
        }
    }
}
