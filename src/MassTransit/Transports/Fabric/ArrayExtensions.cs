namespace MassTransit.Transports.Fabric
{
    using System;


    static class ArrayExtensions
    {
        public static T[] Copy<T>(this T[] array)
        {
            if ((array?.Length ?? 0) == 0)
                return Array.Empty<T>();

            var result = new T[array.Length];
            Array.Copy(array, result, array.Length);
            return result;
        }

        public static T[] Shuffle<T>(this T[] array)
        {
            if ((array?.Length ?? 0) < 2)
                return array;

            var r = new Random();
            for (var i = array.Length - 1; i > 0; i--)
            {
                var j = r.Next(0, i + 1);

                (array[i], array[j]) = (array[j], array[i]);
            }

            return array;
        }
    }
}
