namespace MassTransit.Diagnostics
{
	using System;
	using System.Text;

	class ReceivedMessageTraceDetailFormatter
	{
		readonly ReceivedMessageTraceDetail _detail;
		string _text;
		int _depth;

		public ReceivedMessageTraceDetailFormatter(ReceivedMessageTraceDetail detail)
		{
			_detail = detail;
		}

		public string Text
		{
			get { return _text ?? (_text = Format()); }
		}

		string Format(DateTime value)
		{
			return value.ToString("yyyy-MM-dd hh:mm:ss.fff");
		}

		string Format()
		{
			var sb = new StringBuilder();

			sb.Append('=', 120).AppendLine();
			sb.AppendFormat("== Received: {0,-60}{1,44} ==", _detail.Id, Format(_detail.StartTime)).AppendLine();
			sb.Append('=', 120).AppendLine();

			AppendMessageHeaders(sb, _detail);
			sb.AppendLine();

			if (_detail.Receivers != null)
			{
				foreach (var receiver in _detail.Receivers)
				{
					sb.Append("  ").Append('+', 118).AppendLine();
					sb.Append("  ").AppendFormat("++ {0,-66}{1,46} ++", receiver.ReceiverType, Format(receiver.StartTime)).AppendLine();
					sb.Append("  ").Append('+', 118).AppendLine();

					Append(sb, "Message Type", receiver.MessageType);

					sb.AppendLine();
				}
			}

			if (_detail.SentMessages != null)
			{
				foreach (var sent in _detail.SentMessages)
				{
					sb.Append("  ").Append('-', 118).AppendLine();
					sb.Append("  ").AppendFormat("-- Sent: {0,-60}{1,46} --", sent.Id, Format(sent.StartTime)).AppendLine();
					sb.Append("  ").Append('-', 118).AppendLine();

					Append(sb, "Endpoint Address", sent.Address);
					AppendMessageHeaders(sb, sent);
					sb.AppendLine();
				}
			}

			return sb.ToString();
		}

		void AppendMessageHeaders(StringBuilder sb, MessageTraceDetail detail)
		{
			Append(sb, "Message Id", detail.MessageId);
			Append(sb, "Source Address", detail.SourceAddress);
			Append(sb, "Input Address", detail.InputAddress);
			Append(sb, "Destination Address", detail.DestinationAddress);
			Append(sb, "Response Address", detail.ResponseAddress);
			Append(sb, "Fault Address", detail.FaultAddress);
			Append(sb, "Network", detail.Network);
			Append(sb, "Retry Count", detail.RetryCount);
			Append(sb, "Expiration Time", detail.ExpirationTime);
			Append(sb, "Content Type", detail.ContentType);
			Append(sb, "Message Type", detail.MessageType);
		}

		static StringBuilder Append<T>(StringBuilder sb, string title, T value)
			where T : class
		{
			if (value == null)
				return sb;

			sb.AppendFormat("  {0,-20}: {1}", title, value);
			sb.AppendLine();

			return sb;
		}

		static StringBuilder Append(StringBuilder sb, string title, int value)
		{
			if (value == default(int))
				return sb;

			sb.AppendFormat("  {0,-20}: {1}", title, value);
			sb.AppendLine();

			return sb;
		}

		static StringBuilder Append(StringBuilder sb, string title, DateTime? value)
		{
			if(!value.HasValue)
				return sb;

			sb.AppendFormat("  {0,-20}: {1}", title, value);
			sb.AppendLine();

			return sb;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}