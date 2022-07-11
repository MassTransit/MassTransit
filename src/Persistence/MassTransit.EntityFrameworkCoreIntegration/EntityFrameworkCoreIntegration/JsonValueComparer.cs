namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Serialization;


    public class JsonValueComparer<T> :
        ValueComparer<T>
        where T : class
    {
        public JsonValueComparer()
            : base((t1, t2) => DoEquals(t1, t2), t => DoGetHashCode(t), t => DoGetSnapshot(t))
        {
        }

        static string Json(T instance)
        {
            return ObjectDeserializer.Serialize(instance);
        }

        static T DoGetSnapshot(T instance)
        {
            if (instance is ICloneable cloneable)
                return (T)cloneable.Clone();

            return ObjectDeserializer.Deserialize<T>(Json(instance));
        }

        static int DoGetHashCode(T instance)
        {
            if (instance is IEquatable<T>)
                return instance.GetHashCode();

            return Json(instance).GetHashCode();
        }

        static bool DoEquals(T left, T right)
        {
            if (left is IEquatable<T> equatable)
                return equatable.Equals(right);

            var result = Json(left).Equals(Json(right));
            return result;
        }
    }
}
