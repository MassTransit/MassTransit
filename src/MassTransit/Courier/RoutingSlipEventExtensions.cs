namespace MassTransit.Courier
{
    using System;
    using Contracts;
    using Util;


    public static class RoutingSlipEventExtensions
    {
        public static T GetResult<T>(this RoutingSlipActivityCompensated source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Data, key);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompensated source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetResult<T>(this RoutingSlipActivityCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Data, key);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetArgument<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Arguments, key);
        }

        public static T GetResult<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Data, key);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetArgument<T>(this RoutingSlipActivityFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Arguments, key);
        }

        public static T GetVariable<T>(this RoutingSlipActivityFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetVariable<T>(this RoutingSlipCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetVariable<T>(this RoutingSlipCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }

        public static T GetVariable<T>(this RoutingSlipFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", nameof(key));

            return ObjectTypeDeserializer.Deserialize<T>(source.Variables, key);
        }
    }
}
