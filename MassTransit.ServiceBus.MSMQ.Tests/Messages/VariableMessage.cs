using System;

namespace MassTransit.ServiceBus.MSMQ.Tests.Messages
{
	[Serializable]
	public class VariableMessage : IMessage
	{
	    private string _name;


        //for xml serialization
        private VariableMessage()
        {
        }

	    public VariableMessage(string name)
	    {
	        _name = name;
	    }


	    public string Name
	    {
	        get { return _name; }
	        set { _name = value; }
	    }
	}
}