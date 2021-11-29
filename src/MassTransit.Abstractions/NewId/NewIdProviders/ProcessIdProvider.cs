namespace MassTransit.NewIdProviders
{
    using System;
    using System.Diagnostics;


    public class CurrentProcessIdProvider :
        IProcessIdProvider
    {
        public byte[] GetProcessId()
        {
            var processId = BitConverter.GetBytes(Process.GetCurrentProcess().Id);

            if (processId.Length < 2)
                throw new InvalidOperationException("Current Process Id is of insufficient length");

            return processId;
        }
    }
}
