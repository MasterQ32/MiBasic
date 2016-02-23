namespace MiBasic.Lexer
{
	public sealed class Token
	{
		public Token(TokenType type, string text, int start, int length)
		{
			this.Type = type;
			this.Text = text;
			this.Start = start;
			this.Length = length;
		}

		public int Length { get; private set; }
		public int Start { get; private set; }
		public string Text { get; private set; }
		public TokenType Type { get; private set; }

		public override string ToString()
		{
			return $"{this.Type}[{this.Text}]";
		}
	}

	public enum TokenType
	{
		Unknown,

		/// <summary>
		/// A number literal
		/// </summary>
		Number,

		/// <summary>
		/// A string literal starting with " and ending with ".
		/// Already unescaped.
		/// </summary>
		String,

		/// <summary>
		/// A word identifier.
		/// Matches "\w+"
		/// </summary>
		Identifier,

		/// <summary>
		/// A whitespace token
		/// </summary>
		Whitespace,

		/// <summary>
		/// An operator token.
		/// </summary>
		Operator,

		/// <summary>
		/// ;
		/// </summary>
		Delimiter,

		/// <summary>
		/// An opening bracket
		/// </summary>
		OpeningBracket,

		/// <summary>
		/// An closing bracket
		/// </summary>
		ClosingBracket,
		
		/// <summary>
		/// A comment, multiline #! !# or single line #
		/// </summary>
		Comment,

		/// <summary>
		/// A simple comma
		/// </summary>
		Comma,

		/// <summary>
		/// The dot character
		/// </summary>
		Subscript,

		/// <summary>
		/// The question mark character
		/// </summary>
		ReturnValue,
	}
}