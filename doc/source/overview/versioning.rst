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



Versioning Existing Message Contracts
-------------------------------------

Consider a command to fetch and cache a local copy of an image from a remote system.

.. sourcecode:: csharp
    :linenos:
    
    public interface FetchRemoteImage
    {
    	Guid CommandId { get; }
    	DateTime Timestamp { get; }
    	Uri ImageSource { get; }
    	string LocalCacheKey { get; }
    }

After the initial deployment, a requirement is added to resize the image to a maximum dimension before saving it to the cache. The new message contract includes the additional property specifying the dimension.

.. sourcecode:: csharp
    :linenos:
    
    public interface FetchRemoteImage
    {
    	Guid CommandId { get; }
    	DateTime Timestamp { get; }
    	Uri ImageSource { get; }
    	string LocalCacheKey { get; }
    	int? MaximumDimension { get; }
    }

By making the _int_ value nullable, commands that are submitted using the original contract can still be accepted as the missing value does not break the new contract. If the value was added as a regular _int_, it would be assigned a default value of zero, which may not convey the right information. String values can also be added as they will be _null_ if the value is not present in the serialized message. The consumer just needs to check if the value is present and process it accordingly.






