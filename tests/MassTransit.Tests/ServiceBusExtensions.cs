namespace MassTransit.Tests
{
    using System;


    /// <summary>
    /// Extension methods pertinent to service bus logic, but on
    /// type <see cref="Type" /> - handles different sorts of reflection
    /// logic.
    /// </summary>
    public static class ServiceBusExtensions
    {
        /// <summary>
        /// Transforms the type of message to a normalized string which can be used
        /// for naming a queue on a transport.
        /// </summary>
        /// <param name="messageType">The message class/interface type</param>
        /// <returns>The normalized name for this type</returns>
        public static string ToMessageName(this Type messageType)
        {
            return MessageUrn.ForType(messageType).ToString();
        }
    }
}
