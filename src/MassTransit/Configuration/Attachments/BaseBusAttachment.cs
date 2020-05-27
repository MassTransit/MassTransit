namespace MassTransit.Attachments
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public abstract class BaseBusAttachment :
        IBusAttachment
    {
        readonly BusAttachmentObservable _observers;

        protected BaseBusAttachment(string name)
            : this(name, new BusAttachmentObservable())
        {
        }

        protected BaseBusAttachment(string name, BusAttachmentObservable observers)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(name);

            Name = name;
            _observers = observers ?? throw new ArgumentNullException(nameof(observers));
        }

        public string Name { get; }

        public async Task Connect(CancellationToken cancellationToken)
        {
            await _observers.PreConnect(this).ConfigureAwait(false);

            try
            {
                await BaseConnect(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _observers.ConnectFaulted(e);
                throw;
            }

            await _observers.PostConnect(this);
        }

        public async Task Disconnect(CancellationToken cancellationToken)
        {
            await _observers.PreDisconnect(this).ConfigureAwait(false);

            try
            {
                await BaseDisconnect(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _observers.DisconnectFaulted(e);
                throw;
            }

            await _observers.PostDisconnect(this);
        }

        protected abstract Task BaseConnect(CancellationToken cancellationToken);
        protected abstract Task BaseDisconnect(CancellationToken cancellationToken);
    }
}
