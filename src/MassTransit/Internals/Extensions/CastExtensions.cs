namespace MassTransit.Internals.Extensions
{
    using System;


    public static class CastExtensions
    {
        public static T CastAs<T>(this object obj)
            where T : class
        {
            var self = obj as T;
            if (self == null)
            {
                string message = string.Format("Failed to cast {0} to {1}",
                    obj.GetType().FullName, typeof(T).FullName);
                throw new InvalidOperationException(message);
            }

            return self;
        }
    }
}