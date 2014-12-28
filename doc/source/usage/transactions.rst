Transactions
============

Transactions in a TPL world might be a bit different than what you are used to.


.. sourcecode:: csharp

    public class AConsumer : IConsumer<object>
    {
        public async Task Consume(ConsumeContext<object> context)
        {
            //do stuff
        }
    }
