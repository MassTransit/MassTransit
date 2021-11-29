namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Util.Scanning;


    class RegistrationFilter :
        IRegistrationFilter
    {
        readonly CompositeFilter<Type> _filter;

        public RegistrationFilter(CompositeFilter<Type> filter)
        {
            _filter = filter;
        }

        public bool Matches(IConsumerRegistration registration)
        {
            return _filter.Matches(registration.Type);
        }

        public bool Matches(ISagaRegistration registration)
        {
            return _filter.Matches(registration.Type);
        }

        public bool Matches(IExecuteActivityRegistration registration)
        {
            return _filter.Matches(registration.Type);
        }

        public bool Matches(IActivityRegistration registration)
        {
            return _filter.Matches(registration.Type);
        }

        public bool Matches(IFutureRegistration registration)
        {
            return _filter.Matches(registration.Type);
        }
    }
}
