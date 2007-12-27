using System;
using System.Collections.Generic;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class MessageTypeAddressBook :
        IAddressBook
    {
        private readonly Dictionary<Type, IEndpoint> _typeToDeliveryAddress = new Dictionary<Type, IEndpoint>();

        public MessageTypeAddressBook(IDictionary<string, IEndpoint> addresses)
        {
            if (addresses != null)
            {
                foreach (string typeName in addresses.Keys)
                {
                    Type t = Type.GetType(typeName, true);

                    _typeToDeliveryAddress[t] = addresses[typeName];
                }
            }
        }

        //TODO: consider some add method?

        public IEndpoint Resolve(Type t)
        {
            Check.Require(_typeToDeliveryAddress.ContainsKey(t), string.Format("Unable to find a subscription for {0}", t));

            return _typeToDeliveryAddress[t];
        }
    }
}