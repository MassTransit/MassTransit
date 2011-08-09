namespace MassTransit.Context
{
	using System.Web;

	public class HttpContextContextStorageProvider :
		ContextStorageProvider
	{
		const string ReceiveContextKey = "MassTransitReceiveContext";
		const string SendContextKey = "MassTransitSendContext";

		ContextStorageProvider _fallback;

		public HttpContextContextStorageProvider()
		{
			_fallback = new ThreadStaticContextStorageProvider();
		}

		public ISendContext SendContext
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					var value = httpContext.Items[SendContextKey] as ISendContext;
					return value;
				}

				return _fallback.SendContext;
			}

			set
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					httpContext.Items[SendContextKey] = value;
				}
				else
				{
					_fallback.SendContext = value;
				}
			}
		}

		public IConsumeContext ConsumeContext
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					var value = httpContext.Items[ReceiveContextKey] as IConsumeContext;
					return value;
				}

				return _fallback.ConsumeContext;
			}

			set
			{
				HttpContext httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					httpContext.Items[ReceiveContextKey] = value;
				}
				else
				{
					_fallback.ConsumeContext = value;
				}
			}
		}
	}
}