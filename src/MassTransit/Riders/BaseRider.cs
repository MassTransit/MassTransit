namespace MassTransit.Riders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;


    public abstract class BaseRider :
        IRider
    {
        readonly string _name;
        readonly RiderObservable _observers;

        protected BaseRider(string name)
            : this(name, new RiderObservable())
        {
        }

        protected BaseRider(string name, RiderObservable observers)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(name);

            _name = name;
            _observers = observers ?? throw new ArgumentNullException(nameof(observers));
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            LogContext.Current.Info?.Log("Starting rider: {Name}", _name);
            await _observers.PreStart(this).ConfigureAwait(false);

            try
            {
                await BaseStart(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Current.Error?.Log(exception, "Rider start failed: {Name}", _name);
                await _observers.StartFaulted(exception);
                throw;
            }

            await _observers.PostStart(this);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            LogContext.Current.Info?.Log("Stopping rider: {Name}", _name);
            await _observers.PreStop(this).ConfigureAwait(false);

            try
            {
                await BaseStop(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Current.Error?.Log(exception, "Rider stop failed: {Name}", _name);
                await _observers.StopFaulted(exception);
                throw;
            }

            await _observers.PostStop(this);
        }

        protected abstract Task BaseStart(CancellationToken cancellationToken);
        protected abstract Task BaseStop(CancellationToken cancellationToken);
    }
}
