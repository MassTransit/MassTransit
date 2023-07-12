namespace MassTransit.AmazonS3.MessageData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;
    using Amazon.S3.Util;
    using Util;


    public class AmazonS3MessageDataRepository :
        IMessageDataRepository,
        IBusObserver
    {
        const string RuleName = "s3-messagedata-rule";
        readonly string _bucket;
        readonly IAmazonS3 _s3Client;

        public AmazonS3MessageDataRepository(string bucket)
            : this(new AmazonS3Client(), bucket)
        {
        }

        public AmazonS3MessageDataRepository(AmazonS3Config config, string bucket)
            : this(new AmazonS3Client(config), bucket)
        {
        }

        public AmazonS3MessageDataRepository(IAmazonS3 client, string bucket)
        {
            _s3Client = client;
            _bucket = bucket;
        }

        public void PostCreate(IBus bus)
        {
        }

        public void CreateFaulted(Exception exception)
        {
        }

        public Task PreStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return Task.CompletedTask;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public async Task PreStart(IBus bus)
        {
            try
            {
                var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _bucket);
                if (!bucketExists)
                {
                    try
                    {
                        await _s3Client.PutBucketAsync(new PutBucketRequest
                        {
                            BucketName = _bucket,
                            UseClientRegion = true
                        });
                    }
                    catch (AmazonS3Exception exception)
                    {
                        LogContext.Warning?.Log(exception, "Amazon S3 Bucket does not exist: {Address}", _bucket);
                    }
                }

                if (MessageDataDefaults.TimeToLive != null && MessageDataDefaults.TimeToLive.Value.Days > 0)
                {
                    // Do no delete life cycle rule if TimeToLive is not available. Allow user to create rule in S3 console.
                    await _s3Client.DeleteLifecycleConfigurationAsync(_bucket);
                    await _s3Client.PutLifecycleConfigurationAsync(new PutLifecycleConfigurationRequest
                    {
                        BucketName = _bucket,
                        Configuration = new LifecycleConfiguration
                        {
                            Rules = new List<LifecycleRule>
                            {
                                new LifecycleRule
                                {
                                    Id = RuleName,
                                    Expiration = new LifecycleRuleExpiration { Days = MessageDataDefaults.TimeToLive.Value.Days }
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "S3 Storage failure.");
            }
        }

        public Task PostStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public async Task<Stream> Get(Uri address, CancellationToken cancellationToken = new CancellationToken())
        {
            var filePath = ParseFilePath(address);
            var transferUtility = new TransferUtility(_s3Client);
            return await transferUtility.OpenStreamAsync(_bucket, filePath, cancellationToken);
        }

        public async Task<Uri> Put(Stream stream, TimeSpan? timeToLive = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var filePath = FormatUtil.Formatter.Format(NewId.Next().ToSequentialGuid().ToByteArray());
            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(stream, _bucket, filePath, cancellationToken);
            return new Uri($"urn:file:{filePath.Replace(Path.DirectorySeparatorChar, ':')}");
        }

        static string ParseFilePath(Uri address)
        {
            if (address.Scheme != "urn")
                throw new ArgumentException("The address must be a urn");

            var parts = address.Segments[0].Split(':');
            if (parts[0] != "file")
                throw new ArgumentException("The address must be a urn:file");

            var length = parts.Length - 1;
            var elements = new string[length];
            Array.Copy(parts, 1, elements, 0, length);

            return Path.Combine(elements);
        }
    }
}
