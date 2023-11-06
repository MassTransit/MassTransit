namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Text.RegularExpressions;


    public class AmazonSnsTopicNameValidator :
        IEntityNameValidator
    {
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$", RegexOptions.Compiled);

        public static IEntityNameValidator Validator => Cached.EntityNameValidator;

        public void ThrowIfInvalidEntityName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new AmazonSqsTransportConfigurationException("The entity name must not be null or empty");

            var success = IsValidEntityName(name);
            if (!success)
            {
                throw new AmazonSqsTransportConfigurationException(
                    "The entity name must be a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.");
            }

            if (name.Length > 256)
                throw new AmazonSqsTransportConfigurationException("The topic name length must be <= 256");
        }

        public bool IsValidEntityName(string name)
        {
            return _regex.Match(name).Success;
        }


        static class Cached
        {
            internal static readonly IEntityNameValidator EntityNameValidator = new AmazonSnsTopicNameValidator();
        }
    }
}
