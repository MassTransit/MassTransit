namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Middleware;


    public class RiderCollection :
        Agent,
        IRiderCollection
    {
        readonly Dictionary<string, Handle> _handles;
        readonly object _mutateLock = new object();
        readonly Dictionary<string, IRiderControl> _riders;

        public RiderCollection()
        {
            _riders = new Dictionary<string, IRiderControl>(StringComparer.OrdinalIgnoreCase);
            _handles = new Dictionary<string, Handle>(StringComparer.OrdinalIgnoreCase);
        }

        public IRider Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"The {nameof(name)} must not be null or empty", nameof(name));

            lock (_mutateLock)
            {
                if (!_riders.ContainsKey(name))
                    throw new ConfigurationException($"A rider with the key was not found: {name}");

                if (!_handles.TryGetValue(name, out var handle))
                    throw new ConfigurationException($"A rider has not yet been started: {name}");

                return handle.Rider;
            }
        }

        public void Add(string name, IRiderControl rider)
        {
            if (rider == null)
                throw new ArgumentNullException(nameof(rider));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"The {nameof(name)} must not be null or empty", nameof(name));

            lock (_mutateLock)
            {
                if (_riders.ContainsKey(name))
                    throw new ConfigurationException($"A rider with the same key was already added: {name}");

                _riders.Add(name, rider);
            }
        }

        public HostRiderHandle[] StartRiders(CancellationToken cancellationToken = default)
        {
            KeyValuePair<string, IRiderControl>[] ridersToStart;
            lock (_mutateLock)
                ridersToStart = _riders.Where(x => !_handles.ContainsKey(x.Key)).ToArray();

            return ridersToStart.Select(x => StartRider(x.Key, x.Value, cancellationToken)).ToArray();
        }

        public HostRiderHandle StartRider(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"The {nameof(name)} must not be null or empty", nameof(name));

            IRiderControl rider;
            lock (_mutateLock)
            {
                if (!_riders.TryGetValue(name, out rider))
                    throw new ConfigurationException($"A rider with the key was not found: {name}");

                if (_handles.ContainsKey(name))
                    throw new ArgumentException($"The specified rider has already been started: {name}", nameof(name));
            }

            return StartRider(name, rider, cancellationToken);
        }

        HostRiderHandle StartRider(string name, IRiderControl rider, CancellationToken cancellationToken)
        {
            try
            {
                static async Task<RiderReady> Ready(RiderHandle r, string n)
                {
                    await r.Ready.ConfigureAwait(false);

                    return new ReadyEvent(n);
                }

                var riderHandle = rider.Start(cancellationToken);

                var handle = new Handle(riderHandle, rider, Ready(riderHandle, name), () => Remove(name));

                lock (_mutateLock)
                    _handles.Add(name, handle);

                return handle;
            }
            catch
            {
                lock (_mutateLock)
                    _riders.Remove(name);

                throw;
            }
        }

        void Remove(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"The {nameof(name)} must not be null or empty", nameof(name));

            lock (_mutateLock)
            {
                _riders.Remove(name);
                _handles.Remove(name);
            }
        }

        protected override async Task StopAgent(StopContext context)
        {
            KeyValuePair<string, Handle>[] handles;
            lock (_mutateLock)
                handles = _handles.ToArray();

            await Task.WhenAll(handles.Select(x => x.Value.StopAsync(false, context.CancellationToken))).ConfigureAwait(false);

            await base.StopAgent(context).ConfigureAwait(false);

            lock (_mutateLock)
            {
                foreach (KeyValuePair<string, Handle> handle in handles)
                    _handles.Remove(handle.Key);
            }
        }


        class Handle :
            HostRiderHandle
        {
            readonly Action _remove;
            readonly RiderHandle _riderHandle;
            bool _stopped;

            public Handle(RiderHandle riderHandle, IRider rider, Task<RiderReady> ready, Action remove)
            {
                Rider = rider;
                Ready = ready;
                _riderHandle = riderHandle;
                _remove = remove;
            }

            public IRider Rider { get; }

            public Task<RiderReady> Ready { get; }

            public Task StopAsync(CancellationToken cancellationToken = default)
            {
                return StopAsync(true, cancellationToken);
            }

            public async Task StopAsync(bool remove, CancellationToken cancellationToken = default)
            {
                if (_stopped)
                    return;

                await _riderHandle.StopAsync(cancellationToken).ConfigureAwait(false);

                if (remove)
                    _remove();

                _stopped = true;
            }
        }


        class ReadyEvent :
            RiderReady
        {
            public ReadyEvent(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}
