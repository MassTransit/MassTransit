namespace MassTransit.Testing.Implementations
{
    using System;

    class BusInactivePredicateHelper
    {
        public Func<bool> BusInactivePredicate { get; set; }

        public bool BusInactive()
        {
            return BusInactivePredicate?.Invoke() ?? false;
        }
    }
}
