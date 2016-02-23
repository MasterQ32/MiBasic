using System;
using System.Runtime.Serialization;
using MiBasic.Lexer;

namespace MiBasic.Parser
{
	[Serializable]
	internal class ParserException : Exception
	{
		public ParserException(Token token)
		{
			this.Token = token;
		}

		public ParserException(Token token, string message) : base(message)
		{
			this.Token = token;
		}

		public ParserException(Token token, string message, Exception innerException) : base(message, innerException)
		{
			this.Token = token;
		}

		protected ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public Token Token { get; private set; }
	}
}