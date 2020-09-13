namespace MassTransit.Registration
{
    using System;
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
            return _filter.Matches(registration.ConsumerType);
        }

        public bool Matches(ISagaRegistration registration)
        {
            return _filter.Matches(registration.SagaType);
        }

        public bool Matches(IExecuteActivityRegistration registration)
        {
            return _filter.Matches(registration.ActivityType);
        }

        public bool Matches(IActivityRegistration registration)
        {
            return _filter.Matches(registration.ActivityType);
        }
    }
}
