namespace MassTransit
{
    using System;
    using System.Text;


    public static class MessageDefaults
    {
        static readonly Lazy<Encoding> _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true));

        public static Encoding Encoding => _encoding.Value;
    }
}
