Handling errors
===============

Handling errors in your consumers is quite easy.

Let's suppose that you have a message containing a some data that needs to be
saved to a database. In the event of a failure to save that data, that code
throws an exception say, ``SqlErrorException`` or someother monkey business.
What do you do?

The easiest thing is to just let the exception bubble out of your consumer method
and MassTransit will automatically catch the exception for you, and then, by
default, will retry the message 5 times for you. After 5 attempts it will move
the error to the 'error queue'. From there you will need to inspect the message
in the error queue and decide what you need to do.
