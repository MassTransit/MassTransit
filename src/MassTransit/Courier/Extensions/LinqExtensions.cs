namespace MassTransit.Courier.Extensions
{
    using System.Collections.Generic;


    static class LinqExtensions
    {
        internal static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
        {
            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var element = enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        yield return element;
                        element = enumerator.Current;
                    }
                }
            }
        }
    }
}
