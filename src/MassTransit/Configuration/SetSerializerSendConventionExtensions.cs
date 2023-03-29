namespace MassTransit
{
    using System;
    using System.Net.Mime;
    using Configuration;


    public static class SetSerializerSendConventionExtensions
    {
        /// <summary>
        /// Use the message serializer identified by the specified content type to serialize messages of this type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="contentType"></param>
        public static void UseSerializer<T>(this IMessageSendTopologyConfigurator<T> configurator, ContentType contentType)
            where T : class
        {
            configurator.AddOrUpdateConvention<ISetSerializerMessageSendTopologyConvention<T>>(
                () =>
                {
                    var convention = new SetSerializerMessageSendTopologyConvention<T>();
                    convention.SetSerializer(contentType);

                    return convention;
                },
                update =>
                {
                    update.SetSerializer(contentType);

                    return update;
                });
        }

        /// <summary>
        /// Use the message serializer identified by the specified content type to serialize messages of this type
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="contentType"></param>
        public static void UseSerializer<T>(this IMessageSendTopologyConfigurator<T> configurator, string contentType)
            where T : class
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            var type = new ContentType(contentType);

            UseSerializer(configurator, type);
        }
    }
}
