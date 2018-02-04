namespace MassTransit.Tests.Initializers
{
    using System.Threading;
    using System.Threading.Tasks;
    using InitializerTestMessages;
    using MassTransit.Initializers;
    using MassTransit.Initializers.Contexts;
    using MassTransit.Initializers.Factories;
    using MassTransit.Initializers.PropertyConverters;
    using MassTransit.Initializers.PropertyInitializers;
    using MassTransit.Initializers.TypeConverters;
    using NUnit.Framework;


    [TestFixture]
    public class Testing_the_type_converter_cache
    {
        [Test]
        public void No_converter_for_same_type()
        {
            Assert.That(PropertyInitializerCache.TryGetFactory<int>(typeof(int), out var converter), Is.False);
        }

        [Test]
        public void Non_nullable_to_wider_type()
        {
            Assert.That(PropertyInitializerCache.TryGetFactory<long>(typeof(int), out var converter), Is.True);
            Assert.That(converter.IsPropertyTypeConverter<int>(out var typeConverter), Is.True);
            Assert.That(typeConverter, Is.TypeOf<LongTypeConverter>());
        }

        [Test]
        public void Non_nullable_to_nullable_same_type()
        {
            Assert.That(PropertyInitializerCache.TryGetFactory<int?>(typeof(int), out var converter), Is.True);
            Assert.That(converter.IsPropertyTypeConverter<int>(out var typeConverter), Is.True);
            Assert.That(typeConverter, Is.TypeOf<ToNullableTypeConverter<int>>());

            Assert.That(typeConverter.TryConvert(37, out int? result), Is.True);
            Assert.That(result.Value, Is.EqualTo(37));
        }

        [Test]
        public void Non_nullable_to_nullable_wider_type()
        {
            Assert.That(PropertyInitializerCache.TryGetFactory<long?>(typeof(int), out var converter), Is.True);
            Assert.That(converter.IsPropertyTypeConverter<int>(out var typeConverter), Is.True);
            Assert.That(typeConverter, Is.TypeOf<ToNullableTypeConverter<long, int>>());

            Assert.That(typeConverter.TryConvert(37, out long? result), Is.True);
            Assert.That(result.Value, Is.EqualTo(37));
        }

        [Test]
        public void Nullable_to_non_nullable_wider_type()
        {
            Assert.That(PropertyInitializerCache.TryGetFactory<long>(typeof(int?), out var converter), Is.True);
            Assert.That(converter.IsPropertyTypeConverter<int?>(out var typeConverter), Is.True);
            Assert.That(typeConverter, Is.TypeOf<FromNullableTypeConverter<long, int>>());

            Assert.That(typeConverter.TryConvert(37, out long result), Is.True);
            Assert.That(result, Is.EqualTo(37));
        }

        [Test]
        public void Nullable_to_non_nullable()
        {
            Assert.That(PropertyInitializerCache.TryGetFactory<int>(typeof(int?), out var converter), Is.True);
            Assert.That(converter.IsPropertyTypeConverter<int?>(out var typeConverter), Is.True);
            Assert.That(typeConverter, Is.TypeOf<FromNullableTypeConverter<int>>());

            Assert.That(typeConverter.TryConvert(37, out int result), Is.True);
            Assert.That(result, Is.EqualTo(37));
        }

        [Test]
        public async Task Task_nullable_to_non_nullable()
        {
            Assert.That(PropertyInitializerCache.TryGetFactory<int>(typeof(Task<int?>), out var converter), Is.True);
            Assert.That(converter.IsMessagePropertyConverter<Task<int?>>(out var typeConverter), Is.True);
            Assert.That(typeConverter, Is.TypeOf<AsyncConvertPropertyConverter<int, int?>>());

            var context = MessageFactoryCache<SimpleRequest>.Factory.Create(new BaseInitializeContext(CancellationToken.None));

            var result = await typeConverter.Convert(context, Task.FromResult((int?)37));
            Assert.That(result, Is.EqualTo(37));
        }
    }
}