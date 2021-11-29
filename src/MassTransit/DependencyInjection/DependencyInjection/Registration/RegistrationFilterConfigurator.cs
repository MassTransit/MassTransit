namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Linq;
    using Configuration;
    using Util.Scanning;


    public class RegistrationFilterConfigurator :
        IRegistrationFilterConfigurator
    {
        readonly CompositeFilter<Type> _filter;

        public RegistrationFilterConfigurator()
        {
            _filter = new CompositeFilter<Type>();

            Filter = new RegistrationFilter(_filter);
        }

        public IRegistrationFilter Filter { get; }

        public void Include(params Type[] types)
        {
            _filter.Includes += type => types.Any(x => x == type);
        }

        public void Include<T>()
        {
            _filter.Includes += type => type == typeof(T);
        }

        public void Exclude(params Type[] types)
        {
            _filter.Excludes += type => types.Any(x => x == type);
        }

        public void Exclude<T>()
        {
            _filter.Excludes += type => type == typeof(T);
        }
    }
}
