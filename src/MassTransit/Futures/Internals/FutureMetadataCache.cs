namespace MassTransit.Futures.Internals
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using GreenPipes.Internals.Extensions;


    interface IFutureMetadataCache<T>
        where T : class
    {
        Guid TypeId { get; }
    }


    public class FutureMetadataCache<T> :
        IFutureMetadataCache<T>
        where T : class
    {
        readonly Guid _typeId;

        FutureMetadataCache()
        {
            _typeId = GenerateTypeId();
        }

        public static Guid TypeId => Cached.Metadata.Value.TypeId;

        Guid IFutureMetadataCache<T>.TypeId => _typeId;

        static Guid GenerateTypeId()
        {
            var shortName = TypeCache<T>.ShortName;

            using var hasher = MD5.Create();

            var data = hasher.ComputeHash(Encoding.UTF8.GetBytes(shortName));

            return new Guid(data);
        }


        static class Cached
        {
            internal static readonly Lazy<IFutureMetadataCache<T>> Metadata = new Lazy<IFutureMetadataCache<T>>(() =>
                new FutureMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
