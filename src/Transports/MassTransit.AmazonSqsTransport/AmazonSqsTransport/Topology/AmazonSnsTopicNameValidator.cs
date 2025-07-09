namespace MassTransit.AmazonSqsTransport.Topology;

using System.Text.RegularExpressions;


public class AmazonSnsTopicNameValidator :
    IEntityNameValidator
{
    static readonly Regex _regex = new(@"^[A-Za-z0-9\-_\.:]+$", RegexOptions.Compiled);

    public static IEntityNameValidator Validator => Cached.EntityNameValidator;

    public void ThrowIfInvalidEntityName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new AmazonSqsTransportConfigurationException("The entity name must not be null or empty");

        var success = IsValidEntityName(name);
        if (!success)
        {
            throw new AmazonSqsTransportConfigurationException(
                "The entity name length must be <= 256 and a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.");
        }
    }

    public bool IsValidEntityName(string name)
    {
        return _regex.Match(name).Success && name.Length <= 256;
    }


    static class Cached
    {
        internal static readonly IEntityNameValidator EntityNameValidator = new AmazonSnsTopicNameValidator();
    }
}
