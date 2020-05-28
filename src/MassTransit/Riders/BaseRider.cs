namespace MassTransit.Riders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public abstract class BaseRider :
        IRider
    {
        readonly RiderObservable _observers;

        protected BaseRider(string name)
            : this(name, new RiderObservable())
        {
        }

        protected BaseRider(string name, RiderObservable observers)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(name);

            Name = name;
            _observers = observers ?? throw new ArgumentNullException(nameof(observers));
        }

        public string Name { get; }

        public async Task Start(CancellationToken cancellationToken)
        {
            await _observers.PreStart(this).ConfigureAwait(false);

            try
            {
                await BaseStart(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _observers.StartFaulted(e);
                throw;
            }

            await _observers.PostStart(this);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            await _observers.PreStop(this).ConfigureAwait(false);

            try
            {
                await BaseStop(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _observers.StopFaulted(e);
                throw;
            }

            await _observers.PostStop(this);
        }

        protected abstract Task BaseStart(CancellationToken cancellationToken);
        protected abstract Task BaseStop(CancellationToken cancellationToken);
    }
}
