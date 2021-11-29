namespace MassTransit.NewIdProviders
{
    using System;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;


    public class HostNameHashWorkerIdProvider :
        IWorkerIdProvider
    {
        public byte[] GetWorkerId(int index)
        {
            return GetNetworkAddress();
        }

        static byte[] GetNetworkAddress()
        {
            try
            {
                var hostName = Dns.GetHostName();

                byte[] hash;
                using (var hasher = SHA1.Create())
                {
                    hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(hostName));
                }

                var bytes = new byte[6];
                Buffer.BlockCopy(hash, 12, bytes, 0, 6);
                bytes[0] |= 0x80;

                return bytes;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve hostname", ex);
            }
        }
    }
}
