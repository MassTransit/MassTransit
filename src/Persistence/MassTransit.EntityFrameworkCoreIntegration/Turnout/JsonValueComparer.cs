namespace MassTransit.EntityFrameworkCoreIntegration.Turnout
{
    using System;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Newtonsoft.Json;


    public class JsonValueComparer<T> :
        ValueComparer<T>
    {
        public JsonValueComparer()
            : base((t1, t2) => DoEquals(t1, t2), t => DoGetHashCode(t), t => DoGetSnapshot(t))
        {
        }

        static string Json(T instance)
        {
            return JsonConvert.SerializeObject(instance);
        }

        static T DoGetSnapshot(T instance)
        {
            if (instance is ICloneable cloneable)
                return (T)cloneable.Clone();

            var result = (T)JsonConvert.DeserializeObject(Json(instance), typeof(T));
            return result;
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
