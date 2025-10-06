namespace MassTransit.JobService
{
    using System;
    using System.Security.Cryptography;
    using System.Text;


    public static class JobMetadataCache<TConsumer, TJob>
        where TConsumer : class
        where TJob : class
    {
        public static Guid GenerateJobTypeId(string queueName)
        {
            var key = GenerateJobTypeName(queueName);

            return JobMetadataCache.GenerateHashGuid(key);
        }

        public static string GenerateJobTypeName(string queueName)
        {
            var consumerTypeName = TypeCache<TConsumer>.ShortName;
            var jobTypeName = TypeCache<TJob>.ShortName;

            var name = $"{consumerTypeName}:{jobTypeName}:{queueName}";

            return name;
        }
    }


    public static class JobMetadataCache<TJob>
        where TJob : class
    {
        public static Guid GenerateRecurringJobId(string jobName)
        {
            var key = GenerateJobTypeName(jobName);

            return JobMetadataCache.GenerateHashGuid(key);
        }

        public static string GenerateJobTypeName(string jobName)
        {
            var jobTypeName = TypeCache<TJob>.ShortName;

            var name = $"{jobTypeName}:{jobName}";

            return name;
        }
    }


    static class JobMetadataCache
    {
        static bool? _fipsMode;

        public static bool IsFipsMode =>
            CryptoConfig.AllowOnlyFipsAlgorithms ||
            (_fipsMode ??= bool.TryParse(Environment.GetEnvironmentVariable("MT_FIPS_ENABLE"), out var fipsMode) && fipsMode);

        public static Guid GenerateHashGuid(string key)
        {
            if (IsFipsMode)
            {
                using var hasher = SHA256.Create();

                var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(key));

                return new Guid(new ReadOnlySpan<byte>(data, 0, 16).ToArray());
            }
            else
            {
                using var hasher = MD5.Create();

                var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(key));

                return new Guid(data);
            }
        }
    }
}
