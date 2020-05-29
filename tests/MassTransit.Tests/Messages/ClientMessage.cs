namespace MassTransit.Tests
{
    using System;


    [Serializable]
    public class ClientMessage
    {
        string _name;

        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
}
