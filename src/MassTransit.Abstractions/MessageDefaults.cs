namespace MassTransit
{
    using System;
    using System.Text;
    using System.Threading;


    public static class MessageDefaults
    {
        static readonly Lazy<Encoding> _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true), LazyThreadSafetyMode.PublicationOnly);

        public static Encoding Encoding => _encoding.Value;
    }
}
