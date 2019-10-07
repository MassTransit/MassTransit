namespace MassTransit.Context
{
    public class PublishContextProxy :
        SendContextProxy,
        PublishContext
    {
        readonly SendContext _context;

        public PublishContextProxy(SendContext context)
            : base(context)
        {
            _context = context;
        }

        bool PublishContext.Mandatory { get; set; }

        SendContext<T> SendContext.CreateProxy<T>(T message)
        {
            return new PublishContextProxy<T>(_context, message);
        }
    }


    public class PublishContextProxy<TMessage> :
        SendContextProxy<TMessage>,
        PublishContext<TMessage>
        where TMessage : class
    {
        readonly SendContext _context;

        public PublishContextProxy(SendContext context, TMessage message)
            : base(context, message)
        {
            _context = context;
        }

        bool PublishContext.Mandatory { get; set; }

        SendContext<T> SendContext.CreateProxy<T>(T message)
        {
            return new PublishContextProxy<T>(_context, message);
        }
    }
}
