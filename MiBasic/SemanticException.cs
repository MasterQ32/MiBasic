using System;
using System.Runtime.Serialization;

namespace MiBasic
{
	[Serializable]
	internal class SemanticException : Exception
	{
		public SemanticException()
		{
		}

		public SemanticException(string message) : base(message)
		{
		}

		public SemanticException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected SemanticException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}