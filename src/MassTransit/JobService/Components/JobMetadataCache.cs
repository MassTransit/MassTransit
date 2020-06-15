namespace MassTransit.JobService.Components
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Metadata;


    public static class JobMetadataCache<TJob>
        where TJob : class
    {
        static readonly Lazy<Guid> _jobTypeId = new Lazy<Guid>(() => GenerateJobTypeId());

        public static Guid JobTypeId => _jobTypeId.Value;

        static Guid GenerateJobTypeId()
        {
            var shortName = TypeMetadataCache<TJob>.ShortName;

            using var hasher = MD5.Create();

            byte[] data = hasher.ComputeHash(Encoding.UTF8.GetBytes(shortName));

            return new Guid(data);
        }
    }
}
