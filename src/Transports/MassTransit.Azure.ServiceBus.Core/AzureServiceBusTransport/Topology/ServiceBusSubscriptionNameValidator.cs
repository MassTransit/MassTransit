namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Text.RegularExpressions;


    public class ServiceBusSubscriptionNameValidator :
        IEntityNameValidator
    {
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.]+$", RegexOptions.Compiled);

        public static IEntityNameValidator Validator => Cached.EntityNameValidator;

        public void ThrowIfInvalidEntityName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The entity name must not be null or empty");

            var success = IsValidEntityName(name);
            if (!success)
                throw new ArgumentException("The entity name must be a sequence of these characters: letters, digits, hyphen, underscore, period.");
        }

        public bool IsValidEntityName(string name)
        {
            return _regex.Match(name).Success;
        }


        static class Cached
        {
            internal static readonly IEntityNameValidator EntityNameValidator = new ServiceBusSubscriptionNameValidator();
        }
    }
}
