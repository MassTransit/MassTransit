namespace MassTransit.Host.LifeCycles
{
    using System;
    using Castle.Windsor;

    public interface IApplicationLifeCycle : 
        IDisposable
    {
        void Initialize();
        void Start();
        void Stop();

        //To be fired after disose is complete
        event Action<IApplicationLifeCycle> Completed;

        IWindsorContainer Container { get; }
    }
}