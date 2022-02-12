#nullable disable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public class FutureState :
        SagaStateMachineInstance,
        ISagaVersion
    {
        Dictionary<Guid, FutureMessage> _faults;
        HashSet<Guid> _pending;
        Dictionary<Guid, FutureMessage> _results;
        HashSet<FutureSubscription> _subscriptions;
        Dictionary<string, object> _variables;

        public int CurrentState { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Faulted { get; set; }

        public Uri Location { get; set; }

        public FutureMessage Command { get; set; }

        public HashSet<Guid> Pending
        {
            get
            {
                if (_pending != null)
                    return _pending;

                lock (this)
                    _pending ??= new HashSet<Guid>();

                return _pending;
            }
            private set => _pending = value;
        }

        public HashSet<FutureSubscription> Subscriptions
        {
            get
            {
                if (_subscriptions != null)
                    return _subscriptions;

                lock (this)
                    _subscriptions ??= new HashSet<FutureSubscription>(FutureSubscription.Comparer);

                return _subscriptions;
            }
            private set => _subscriptions = value;
        }

        public Dictionary<string, object> Variables
        {
            get
            {
                if (_variables != null)
                    return _variables;

                lock (this)
                    _variables ??= new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                return _variables;
            }
            private set => _variables = value != null ? new Dictionary<string, object>(value, StringComparer.OrdinalIgnoreCase) : null;
        }

        public Dictionary<Guid, FutureMessage> Results
        {
            get
            {
                if (_results != null)
                    return _results;

                lock (this)
                    _results ??= new Dictionary<Guid, FutureMessage>();

                return _results;
            }
            private set => _results = value;
        }

        public Dictionary<Guid, FutureMessage> Faults
        {
            get
            {
                if (_faults != null)
                    return _faults;

                lock (this)
                    _faults ??= new Dictionary<Guid, FutureMessage>();

                return _faults;
            }
            private set => _faults = value;
        }

        public byte[] RowVersion { get; set; }
        public int Version { get; set; }

        public Guid CorrelationId { get; set; }

        public bool HasSubscriptions()
        {
            return _subscriptions != null && _subscriptions.Count > 0;
        }

        public bool HasVariables()
        {
            return _variables != null && _variables.Count > 0;
        }

        public bool HasResults()
        {
            return _results != null && _results.Count > 0;
        }

        public bool HasFaults()
        {
            return _faults != null && _faults.Count > 0;
        }

        public bool HasPending()
        {
            return _pending != null && _pending.Count > 0;
        }
    }
}
