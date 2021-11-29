namespace MassTransitBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class Analytics
    {
        public static double? Median<TColl, TValue>(
            this IEnumerable<TColl> source,
            Func<TColl, TValue> selector)
            where TValue : struct
        {
            return source.Select(selector).Median();
        }

        public static double? Percentile<TColl, TValue>(
            this IEnumerable<TColl> source,
            Func<TColl, TValue> selector, int percentile = 95)
            where TValue : struct
        {
            return source.Select(selector).Percentile(percentile);
        }

        public static double? Median<T>(
            this IEnumerable<T> source)
            where T : struct
        {
            int count = source.Count();
            if (count == 0)
                return null;

            source = source.OrderBy(n => n);

            int midpoint = count / 2;
            if (count % 2 == 0)
            {
                return (Convert.ToDouble(source.ElementAt(midpoint - 1)) + Convert.ToDouble(source.ElementAt(midpoint)))
                    / 2.0;
            }

            return Convert.ToDouble(source.ElementAt(midpoint));
        }

        public static double? Percentile<T>(
            this IEnumerable<T> source, int percentile)
            where T : struct
        {
            int count = source.Count();
            if (count == 0)
                return null;

            source = source.OrderBy(n => n);

            int point = count * percentile / 100;
            if (count % 2 == 0)
            {
                return (Convert.ToDouble(source.ElementAt(point - 1)) + Convert.ToDouble(source.ElementAt(point)))
                    / 2.0;
            }

            return Convert.ToDouble(source.ElementAt(point));
        }
    }
}