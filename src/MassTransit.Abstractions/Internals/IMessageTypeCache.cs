namespace MassTransit.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;


    interface IMessageTypeCache
    {
        /// <summary>
        /// The friendly diagnostic address for display in metrics applications
        /// </summary>
        string DiagnosticAddress { get; }

        /// <summary>
        /// True if the type implements any known saga interfaces
        /// </summary>
        bool HasConsumerInterfaces { get; }

        /// <summary>
        /// True if the type implements any known saga interfaces
        /// </summary>
        bool HasSagaInterfaces { get; }

        /// <summary>
        /// True if the message type is a valid message type
        /// </summary>
        bool IsValidMessageType { get; }

        /// <summary>
        /// Once checked, the reason why the message type is invalid
        /// </summary>
        string? InvalidMessageTypeReason { get; }

        /// <summary>
        /// True if this message is not a public type
        /// </summary>
        bool IsTemporaryMessageType { get; }

        /// <summary>
        /// Returns all valid message types that are contained within the s
        /// </summary>
        Type[] MessageTypes { get; }

        /// <summary>
        /// The names of all the message types supported by the message type
        /// </summary>
        string[] MessageTypeNames { get; }

        IEnumerable<PropertyInfo> Properties { get; }
    }
}
