namespace MassTransit
{
    using System;

    public static class Extensions
    {
		public static MessageUrn ToMessageUrn(this Type type)
		{
			return new MessageUrn(type);
		}
    }
}