using MiBasic.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiBasic.Parser
{
	public class TokenReader
	{
		private readonly Token[] tokens;
		private int cursor = 0;

		public TokenReader(Token[] tokens)
		{
			// Duplicate the array to prevent post-instantiation changes.
			this.tokens = tokens.ToArray();
		}

		public bool EndOfTokens => this.cursor >= this.tokens.Length;

		public IReadOnlyList<Token> Tokens => this.tokens;

		public void Advance(int dist) => this.cursor += dist;

		public int Position => this.cursor;

		public int Length => this.tokens.Length;

		public Token PeekToken()
		{
			return this.tokens[this.cursor];
		}

		public bool CheckForKeyword(Keyword keyword)
		{
			var token = this.PeekToken();
			return GetKeyword(token) == keyword;
		}

		public void ReadCommaSeparatedList(Action readItem)
		{
			this.ReadOpeningBracket();
			var token = this.PeekToken();
			if (token.Type != TokenType.ClosingBracket)
			{
				while (true)
				{
					readItem();
					token = this.PeekToken();
					if (token.Type == TokenType.ClosingBracket)
						break;
					this.ReadComma();
				}
			}
			this.ReadClosingBracket();
		}

		public void ReadSpecificToken(TokenType type)
		{
			var token = this.tokens[this.cursor++];
			if (token.Type != type)
				throw new ParserException(token, $"Expected {type}.");
		}

		public void ReadOpeningBracket() => ReadSpecificToken(TokenType.OpeningBracket);

		public void ReadClosingBracket() => ReadSpecificToken(TokenType.ClosingBracket);

		public void ReadDelimiter() => ReadSpecificToken(TokenType.Delimiter);

		public void ReadComma() => ReadSpecificToken(TokenType.Comma);

		public string ReadIdentifier()
		{
			var token = this.tokens[this.cursor++];
			if (token.Type != TokenType.Identifier)
				throw new ParserException(token, "Expected an identifier.");
			return token.Text;
		}

		public BasicTypeName ReadType()
		{
			var token = this.tokens[this.cursor++];
			if (token.Type != TokenType.Identifier)
				throw new ParserException(token, "Expected a type name.");
			return new BasicTypeName(token.Text);
		}

		public Keyword ReadKeyword()
		{
			var keyword = GetKeyword(this.tokens[this.cursor]);
			if (keyword == Keyword.Invalid)
				throw new ParserException(this.tokens[this.cursor], "Expected a keyword.");
			this.cursor++;
			return keyword;
		}

		public Keyword ReadKeyword(params Keyword[] options)
		{
			var keyword = GetKeyword(this.tokens[this.cursor]);
			if (options.Contains(keyword) == false)
				throw new ParserException(this.tokens[this.cursor], $"Expected {string.Join(" or ", options)}.");
			this.cursor++;
			return keyword;
		}

		public static Keyword GetKeyword(Token token)
		{
			if (token.Type != TokenType.Identifier)
				return Keyword.Invalid;
			foreach (Keyword keyword in Enum.GetValues(typeof(Keyword)))
			{
				if (keyword.ToString().IsSemanticEqualTo(token.Text))
					return keyword;
			}
			return Keyword.Invalid;
		}
	}
}