Your First Consumer
===================

Writing your first consumer in MassTransit. One of the key tenants of MassTransit
is that we need to respect the very asynchronus nature of messaging.

.. sourcecode:: csharp

    public class AConsumer : IConsumer<object>
    {
        public async Task Consume(ConsumeContext<object> context)
        {
            //do stuff
        }
    }


Observers
---------

.. sourcecode:: csharp

    public class AnObserver :
        IObserver<ConsumeContext<MyConsumer>>
    {
        public void OnNext(ConsumeContext<MyConsumer> value)
        {
            //do stuff
        }

        public void OnError(Exception error)
        {
            //error handling
        }

        public void OnCompleted()
        {
            //on complete
        }

    }

