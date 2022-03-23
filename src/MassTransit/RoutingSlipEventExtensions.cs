#nullable enable
namespace MassTransit
{
    using Courier.Contracts;


    public static class RoutingSlipEventExtensions
    {
        public static T? GetVariable<T>(this ConsumeContext<RoutingSlip> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlip> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetResult<T>(this ConsumeContext<RoutingSlipActivityCompensated> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Data, key, defaultValue);
        }

        public static T? GetResult<T>(this ConsumeContext<RoutingSlipActivityCompensated> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Data, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipActivityCompensated> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipActivityCompensated> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetResult<T>(this ConsumeContext<RoutingSlipActivityCompensationFailed> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Data, key, defaultValue);
        }

        public static T? GetResult<T>(this ConsumeContext<RoutingSlipActivityCompensationFailed> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Data, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipActivityCompensationFailed> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipActivityCompensationFailed> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetArgument<T>(this ConsumeContext<RoutingSlipActivityCompleted> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Arguments, key, defaultValue);
        }

        public static T? GetArgument<T>(this ConsumeContext<RoutingSlipActivityCompleted> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Arguments, key, defaultValue);
        }

        public static T? GetResult<T>(this ConsumeContext<RoutingSlipActivityCompleted> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Data, key, defaultValue);
        }

        public static T? GetResult<T>(this ConsumeContext<RoutingSlipActivityCompleted> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Data, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipActivityCompleted> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipActivityCompleted> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetArgument<T>(this ConsumeContext<RoutingSlipActivityFaulted> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Arguments, key, defaultValue);
        }

        public static T? GetArgument<T>(this ConsumeContext<RoutingSlipActivityFaulted> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Arguments, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipActivityFaulted> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipActivityFaulted> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipCompensationFailed> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipCompensationFailed> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipCompleted> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipCompleted> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipFaulted> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipFaulted> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipTerminated> context, string key, T? defaultValue = null)
            where T : class
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }

        public static T? GetVariable<T>(this ConsumeContext<RoutingSlipTerminated> context, string key, T? defaultValue = null)
            where T : struct
        {
            return context.SerializerContext.GetValue(context.Message.Variables, key, defaultValue);
        }
    }
}
