namespace MassTransit
{
    using System;


    /// <summary>
    /// Specify the message type name for this message type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class MessageUrnAttribute :
        Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="urn">The urn value to use for this message type.</param>
        /// <param name="useDefaultPrefix">Prefixes with default scheme and namespace if true.</param>
        public MessageUrnAttribute(string urn, bool useDefaultPrefix = true)
        {
            if (urn == null)
                throw new ArgumentNullException(nameof(urn));

            if (string.IsNullOrWhiteSpace(urn))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(urn));

            if (urn.StartsWith(MessageUrn.Prefix))
                throw new ArgumentException($"Value should not contain the default prefix '{MessageUrn.Prefix}'.", nameof(urn));

            Urn = FormatUrn(urn, useDefaultPrefix);
        }

        public Uri Urn { get; }

        static Uri FormatUrn(string urn, bool useDefaultPrefix)
        {
            var fullValue = useDefaultPrefix ? MessageUrn.Prefix + urn : urn;

            if (Uri.TryCreate(fullValue, UriKind.Absolute, out var uri))
                return uri;

            throw new UriFormatException($"Invalid URN: {fullValue}");
        }
    }
}
