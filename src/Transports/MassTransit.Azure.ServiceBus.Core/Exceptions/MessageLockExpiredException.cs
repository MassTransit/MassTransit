﻿namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class MessageLockExpiredException :
        TransportException
    {
        public MessageLockExpiredException()
        {
        }

        public MessageLockExpiredException(Uri uri)
            : base(uri)
        {
        }

        public MessageLockExpiredException(Uri uri, string message)
            : base(uri, message)
        {
        }

        public MessageLockExpiredException(Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected MessageLockExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
