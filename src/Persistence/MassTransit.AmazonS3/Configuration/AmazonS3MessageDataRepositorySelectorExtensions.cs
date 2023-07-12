namespace MassTransit
{
    using System;
    using Amazon.S3;
    using AmazonS3.MessageData;
    using Configuration;


    public static class AmazonS3MessageDataRepositorySelectorExtensions
    {
        /// <summary>
        /// Use Amazon S3 Storage for message data storage
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="bucket">Bucket for s3.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMessageDataRepository AmazonS3(this IMessageDataRepositorySelector selector, string bucket)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));

            var repository = new AmazonS3MessageDataRepository(bucket);

            return repository;
        }

        /// <summary>
        /// Use Amazon S3 Storage for message data storage
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="bucket">Bucket for s3.</param>
        /// <param name="config">Amazon s3 config.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMessageDataRepository AmazonS3(this IMessageDataRepositorySelector selector, string bucket, AmazonS3Config config)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (string.IsNullOrEmpty(bucket))
                throw new ArgumentNullException(nameof(bucket));

            var repository = new AmazonS3MessageDataRepository(config, bucket);

            return repository;
        }
    }
}
