namespace MassTransit
{
    using System;
    using System.Linq;
    using Internal;
    using Internal.RequestResponse;
    using Services.LoadBalancer;

    public static class ExtensionsToServiceBus
    {
        /// <summary>
        /// Make a request to a service and wait for the response to be received, built
        /// using a fluent interface with a final call to Send()
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="requestAction"></param>
        /// <returns></returns>
        public static RequestResponseScope MakeRequest(this IServiceBus bus, Action<IServiceBus> requestAction)
        {
            return new RequestResponseScope(bus, requestAction);
        }

        public static ResponseActionBuilder<T> When<T>(this RequestResponseScope scope)
            where T : class
        {
            return new ResponseActionBuilder<T>(scope);
        }

        public static IUnsubscribeAction Disposable(this UnsubscribeAction action)
        {
            return new DisposableUnsubscribeAction(action);
        }

        public static void Execute<T>(this IServiceBus bus, T message, Action<IOutboundMessage> action)
            where T : class
        {
            OutboundMessage.Set(action);

            bus.Execute(message);
        }

        public static void Execute<T>(this IServiceBus bus, T message)
            where T : class
        {
            var loadBalancer = bus.GetService<ILoadBalancerService>();

            Action<object>[] consumers = bus.OutboundPipeline.Enumerate(message).ToArray();
            loadBalancer.Execute(message, consumers);
        }

        public static void Publish<T>(this IServiceBus bus, T message, Action<IOutboundMessage> action)
            where T : class
        {
            OutboundMessage.Set(action);

            bus.Publish(message);
        }

        public static void Send<T>(this IEndpoint endpoint, T message, Action<IOutboundMessage> action)
            where T : class
        {
            OutboundMessage.Set(action);

            endpoint.Send(message);
        }

        public static string ToMessageName(this Type messageType)
        {
            string assembly = messageType.Assembly.FullName;
            assembly = assembly.Substring(0, assembly.IndexOf(','));

            return string.Format("{0}, {1}", messageType.FullName, assembly);
        }

        public static string ToFriendlyName(this Type type)
        {
            if (type.IsGenericType)
            {
                string name = type.GetGenericTypeDefinition().FullName;
                name = name.Substring(0, name.IndexOf('`'));
                name += "<";

                Type[] arguments = type.GetGenericArguments();
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (i > 0)
                        name += ",";

                    name += arguments[i].Name;
                }

                name += ">";

                return name;
            }

            return type.FullName;
        }
    }
}