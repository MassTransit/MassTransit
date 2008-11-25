using System;

namespace MassTransit.MSMQ.Tests.Messages
{
	[Serializable]
	public class VariableMessage
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