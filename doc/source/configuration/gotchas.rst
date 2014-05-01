Common Gotcha's
===============

Trying to share a queue
"""""""""""""""""""""""

Each application needs it own address! If you have a website and a console application they will
each need their own address. For instance the website could listen at ``msmq://localhost/web`` and
the console at ``msmq://localhost/console``.

The reason for this is you are defining the 'receiving' queue. This is where the bus instance
will receive its messages. It would be like sharing a mailbox with your neighbor, some times
you get all the mail, sometimes they get all the mail.


How to do an NServiceBus send only endpoint?
""""""""""""""""""""""""""""""""""""""""""""

Use the IEndpointResolver to get an Endpoint that you can call ``.Send(msg)`` on.


How to setup a competing consumer?
""""""""""""""""""""""""""""""""""

need to doc this. ;)

RabbitMQ and Msmq

Where did the Batch<T> go?
""""""""""""""""""""""""""

We removed the concept, as it required you to bind to the MT dll too 
tightly in your messages. We don't yet have a way to reproduce the 
behavior but you can do this in your application. If you would like
to submit an example we would appreciate it. :)

So, what links two bus instances together?
""""""""""""""""""""""""""""""""""""""""""

This is a common question. The binding element, really is the 
message contract. If you want message A, then you subscribe to 
message A. The internals of MT wires it all together.

Why aren't queue / message priorities supported?
""""""""""""""""""""""""""""""""""""""""""""""""

Message Priorities are used to allow a message to jump to the front
of the line. When people ask for this feature they usually have multiple
types of messages all being delivered to the same queue. The problem
is that each message has a different SLA (usually the one with the
shorter time window is the one getting the priority flag). The problem
is that w/o priorities the important message gets stuck behind the 
less important/urgent ones.

The solution is to stop sharing a single queue, and instead establish
a second queue. In MassTransit you would establish a second instance
of IServiceBus and have it subscribe to the important/urgent 
message. Now you have two queues, one for the important things and one
for the less urgent things. This helps with monitoring queue depths,
error rates, etc. By placing each IServiceBus in its own Topshelf host
/ process you further enhance each bus's ability to process messages, and
isolate issues / downtime.

Reading
'''''''

http://www.udidahan.com/2008/01/30/podcast-message-priority-you-arent-gonna-need-it/
http://lostechies.com/jimmybogard/2010/11/18/queues-are-still-queues/

I want to know if another bus is subscribed to my message.
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

    So, if you try to program this way, you're going to have a bad time. ;)

Knowing that you have a subscriber is not the concern of your application.
It is something the system architect should know, but not the application.
Most likely, we just need to introduce all of the states in our protocol
more explicitly, by using a Saga.

A Sample Saga
''''''''''''''

.. code::

    //Magnum.StateMachine based saga
    public class MySaga  :
		SagaStateMachine<MySaga>,
		ISaga
    {
        static MySaga()
        {
            Define(() =>
            {
                Initially(
                    When(CommandOccured)
                        .Then(saga => saga.StartTimer()) //pseudo code
                        .TransitionTo(InProcess));
                
                During(InProcess,
                    When(Timeout)
                        .TransitionTo(TimedOut),
                    When(Succeeded)
                        .TransitionTo(Success),
                    When(Error)
                        .TransitionTo(Failed));
                
            });
        }
        
        //States
        public static State Initial { get; set; }
        public static State InProcess { get; set; }
        public static State TimedOut { get; set; }
        public static State Succeeded { get; set; }
        public static State Error { get; set; }
		public static State Completed { get; set; }
        
        //Events
        public static Event<MyCommand> CommandOccured { get; set; }
		public static Event<TimeoutEvent> Timeout { get; set; }
		public static Event<MyCommandSuccess> Succeeded { get; set; }
		public static Event<MyCommandError> Error { get; set; }
        
        
        //instance data
        public virtual Guid CorrelationId {get;set;}
    }

