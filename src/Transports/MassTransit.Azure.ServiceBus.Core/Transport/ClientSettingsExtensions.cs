namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;


    public static class ClientSettingsExtensions
    {
        public static SessionHandlerOptions GetSessionHandlerOptions(this ClientSettings settings, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            var options = new SessionHandlerOptions(exceptionHandler)
            {
                AutoComplete = false,
                MaxAutoRenewDuration = settings.MaxAutoRenewDuration,
                MaxConcurrentSessions = settings.MaxConcurrentCalls,
                MessageWaitTimeout = settings.MessageWaitTimeout
            };

            return options;
        }

        public static MessageHandlerOptions GetOnMessageOptions(this ClientSettings settings, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            var options = new MessageHandlerOptions(exceptionHandler)
            {
                AutoComplete = false,
                MaxAutoRenewDuration = settings.MaxAutoRenewDuration,
                MaxConcurrentCalls = settings.MaxConcurrentCalls
            };

            return options;
        }
    }
}
