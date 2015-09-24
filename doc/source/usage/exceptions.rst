Handling Exceptions
===================

Handling errors in your consumers is quite easy.

Let's suppose that you have a message containing a some data that needs to be
saved to a database. In the event of a failure to save that data, that code
throws an exception say, ``SqlErrorException`` or some other monkey business.
What do you do?

The easiest thing is to just let the exception bubble out of your consumer method
and MassTransit will automatically catch the exception for you, and then, by
default, will retry the message 5 times for you. After 5 attempts it will move
the error to the 'error queue'. From there you will need to inspect the message
in the error queue and decide what you need to do.

.. sourcecode:: csharp

    public class AConsumer : IConsumer<object>
    {
        public async Task Consume(ConsumeContext<object> context)
        {
            throw new Exception("Something went wrong");
        }
    }

If that isn't fancy enough for you, there is another option as well. You can
handle the error yourself. On the ``ConsumeContex<T>`` class are a number of
methods that you can use.

.. sourcecode:: csharp

    public class AConsumer : IConsumer<object>
    {
        public async Task Consume(ConsumeContext<object> context)
        {
            try
            {
                //do work
            }
            catch (Exception ex)
            {
                context.NotifyFaulted(context.Message, "consumer type", ex);
            }
        }
    }
