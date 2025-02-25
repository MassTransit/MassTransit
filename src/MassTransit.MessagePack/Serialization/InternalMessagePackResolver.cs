namespace MassTransit.Serialization;

using MessagePack;
using MessagePack.Resolvers;


static class InternalMessagePackResolver
{
    static IFormatterResolver InternalResolverInstance { get; } =
        CompositeResolver.Create(NativeDateTimeResolver.Instance,
            ContractlessStandardResolverAllowPrivate.Instance,
            MassTransitMessagePackFormatterResolver.Instance,
            DynamicGenericResolver.Instance);

    public static MessagePackSerializerOptions Options { get; } = MessagePackSerializerOptions.Standard.WithResolver(InternalResolverInstance);
}
