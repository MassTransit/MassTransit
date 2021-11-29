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
            var consumerTypeName = TypeCache<TConsumer>.ShortName;
            var jobTypeName = TypeCache<TJob>.ShortName;

            var key = $"{consumerTypeName}:{jobTypeName}:{queueName}";

            using var hasher = MD5.Create();

            var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(key));

            return new Guid(data);
        }
    }
}
