namespace MassTransit.Util
{
    using System;
    using System.Threading;
    using GreenPipes.Agents;


    /// <summary>
    /// Recycles a supervisor once it is stopped, replacing it with a new one
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Recycle<T>
        where T : class, IAgent
    {
        T _supervisor;

        public Recycle(Func<T> supervisorFactory)
        {
            CancellationTokenRegistration registration = default;

            void RecycleSupervisor()
            {
                registration.Dispose();

                Volatile.Write(ref _supervisor, supervisorFactory());

                registration = _supervisor.Stopping.Register(() => RecycleSupervisor());
            }

            RecycleSupervisor();
        }

        public T Supervisor => Volatile.Read(ref _supervisor);
    }
}
