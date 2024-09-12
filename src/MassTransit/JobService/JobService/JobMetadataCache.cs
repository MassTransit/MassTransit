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

            using var hasher = MD5.Create();

            var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(key));

            return new Guid(data);
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

            using var hasher = MD5.Create();

            var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(key));

            return new Guid(data);
        }

        public static string GenerateJobTypeName(string jobName)
        {
            var jobTypeName = TypeCache<TJob>.ShortName;

            var name = $"{jobTypeName}:{jobName}";

            return name;
        }
    }
}
