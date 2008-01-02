using System;
using System.Diagnostics;

namespace MassTransit.ServiceBus.Util
{
    using System.Transactions;

    /// <summary>
	/// Check class for verifying the condition of items included in interface contracts
	/// </summary>
	public static class Check
	{
		private static bool _useExceptions = true;

		public static bool UseExceptions
		{
			get { return _useExceptions; }
			set { _useExceptions = value; }
		}

		public static ObjectCheck Parameter(object objectUnderCheck)
		{
			return new ObjectCheck(objectUnderCheck);
		}

		public static StringCheck Parameter(string stringUnderCheck)
		{
			return new StringCheck(stringUnderCheck);
		}

		public static void ThrowArgumentNullException(string message)
		{
			if (string.IsNullOrEmpty(message))
				throw new ArgumentNullException();
			else
				throw new ArgumentNullException("", message);
		}

		public static void Require(bool assertion, string message)
		{
			if (UseExceptions)
			{
				if (!assertion) throw new PreconditionException(message);
			}
			else
			{
				Trace.Assert(assertion, "Precondition: " + message);
			}
		}

        public static void RequireTransaction(string message)
        {
            if (Transaction.Current == null)
                throw new Exception("Transaction Required: " + message);
        }

		public static void Ensure(bool assertion, string message)
		{
			if (UseExceptions)
			{
				if (!assertion) throw new PostconditionException(message);
			}
			else
			{
				Trace.Assert(assertion, "Postcondition: " + message);
			}
		}

		#region Nested type: ObjectCheck

		public class ObjectCheck
		{
			private readonly object _objectUndercheck;
			private string _message;

			public ObjectCheck(object objectUnderCheck)
			{
				_objectUndercheck = objectUnderCheck;
			}

			public void IsNotNull()
			{
				if (_objectUndercheck == null)
					ThrowArgumentNullException(_message);
			}

			public ObjectCheck WithMessage(string message)
			{
				_message = message;

				return this;
			}
		}

		#endregion

		#region Nested type: StringCheck

		public class StringCheck
		{
			private readonly string _stringUnderCheck;
			private string _message;

			public StringCheck(string stringUnderCheck)
			{
				_stringUnderCheck = stringUnderCheck;
			}

			public void IsNotNullOrEmpty()
			{
				if (string.IsNullOrEmpty(_stringUnderCheck))
				{
					ThrowArgumentNullException(_message);
				}
			}

			public StringCheck WithMessage(string message)
			{
				_message = message;

				return this;
			}
		}

		#endregion
	}

	public class CheckException : ApplicationException
	{
		protected CheckException()
		{
		}

		protected CheckException(string message) : base(message)
		{
		}

		protected CheckException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	public class PreconditionException : CheckException
	{
		/// <summary>
		/// Precondition Exception.
		/// </summary>
		public PreconditionException()
		{
		}

		/// <summary>
		/// Precondition Exception.
		/// </summary>
		public PreconditionException(string message) : base(message)
		{
		}

		/// <summary>
		/// Precondition Exception.
		/// </summary>
		public PreconditionException(string message, Exception inner) : base(message, inner)
		{
		}
	}

	/// <summary>
	/// Exception raised when a postcondition fails.
	/// </summary>
	public class PostconditionException : CheckException
	{
		/// <summary>
		/// Postcondition Exception.
		/// </summary>
		public PostconditionException()
		{
		}

		/// <summary>
		/// Postcondition Exception.
		/// </summary>
		public PostconditionException(string message) : base(message)
		{
		}

		/// <summary>
		/// Postcondition Exception.
		/// </summary>
		public PostconditionException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}