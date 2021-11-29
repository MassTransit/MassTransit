namespace MassTransit
{
    using Transports;


    public class MessageNameFormatterEntityNameFormatter :
        IEntityNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public MessageNameFormatterEntityNameFormatter(IMessageNameFormatter formatter)
        {
            _formatter = formatter;
        }

        string IEntityNameFormatter.FormatEntityName<T>()
        {
            return _formatter.GetMessageName(typeof(T)).ToString();
        }
    }
}
