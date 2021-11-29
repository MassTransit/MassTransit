namespace MassTransit.Util
{
    /// <summary>
    /// A do-nothing connect handle, simply to satisfy
    /// </summary>
    public class EmptyConnectHandle :
        ConnectHandle
    {
        public void Dispose()
        {
        }

        public void Disconnect()
        {
        }
    }
}
