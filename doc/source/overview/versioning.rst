Versioning Messages
===================

Interfaces are the best way to handle versioning of messages. Your event producer publishes the
class that implements one or more interfaces, and your consumers subscribe to the interfaces.
MassTransit will make sure everyone gets a message with the right interface for the single publish.

.. sourcecode:: csharp
    :linenos:
    
    public class CustomerMessage :
        IBasicCustomerMessage, IEnhancedCustomerMessage
    {
        //implement interfaces
    }

Note that you can't do dynamic casting of the message once it's consumer (such as ``message as
ISomeOtherInterface``) because when you subscribe to an interface using the Xml/Json/Bson serializers,
you get a proxy back instead of your actual class that was published. 