namespace MiBasic
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public sealed class Tokenizer
	{
		private Tokenizer()
		{

		}

		public static Token[] Tokenize(string source)
		{
			return Tokenize(source, EmitOptions.None);
		}

		public static Token[] Tokenize(string source, EmitOptions options)
		{
			var tokens = new List<Token>();
			for (int i = 0; i < source.Length;)
			{
				var token = GetToken(source, i);
				switch (token.Type)
				{
					case TokenType.Whitespace:
					{
						if (options.HasFlag(EmitOptions.EmitWhitespace) == false)
							break;
						goto default;
					}
					case TokenType.Comment:
					{
						if (options.HasFlag(EmitOptions.EmitComments) == false)
							break;
						goto default;
					}
					default:
					{
						tokens.Add(token);
						break;
					}
				}
				i += token.Length;
			}
			return tokens.ToArray();
		}

		private static Token GetToken(string source, int start)
		{
			foreach (var pattern in tokenPatterns)
			{
				var match = pattern.Item1.Match(source, start);
				if (match.Success == false)
					continue;
				if (match.Index != start)
					continue;
				var value = match.Value;
				switch (pattern.Item2)
				{
					case TokenType.String: // Special case, unescape string
					{
						value = value.Substring(1, value.Length - 2);
						value = UnescapeString(value);
						break;
					}
				}
				return new Token(pattern.Item2, value, start, match.Length);
			}
			return new Token(TokenType.Unknown, source.Substring(start), start, source.Length);
		}

		private static string UnescapeString(string value)
		{
			var escapeCodes = new string[][]
			{
				new [] {"\\n", "\n" },
				new [] {"\\r", "\r" },
				new [] {"\\\"", "\"" },
				new [] {"\\'", "\'" },
			};
			foreach (var code in escapeCodes)
			{
				value = value.Replace(code[0], code[1]);
			}
			return value;
		}

		private static Regex R(string text) => new Regex(text, RegexOptions.Compiled);

		private static Regex R(string text, RegexOptions options) => new Regex(text, options | RegexOptions.Compiled);

		private static Tuple<Regex, TokenType> Pattern(string text, TokenType type) => Tuple.Create(R(text), type);

		private static Tuple<Regex, TokenType> Pattern(string text, TokenType type, RegexOptions options) => Tuple.Create(R(text, options), type);

		private static readonly Tuple<Regex, TokenType>[] tokenPatterns = new Tuple<Regex, TokenType>[]
		{
			Pattern(@"\s+", TokenType.Whitespace),
			Pattern(@"#!.*?!#", TokenType.Comment, RegexOptions.Singleline),
			Pattern(@"#.*$", TokenType.Comment, RegexOptions.Multiline),
			Pattern(@"(?:0x[0-9A-Fa-f]+|-?\d*\.?\d+)", TokenType.Number),
			Pattern(@"[A-Za-z_]\w*", TokenType.Identifier),
			Pattern(@"\.", TokenType.Subscript),
			Pattern(@";", TokenType.Delimiter),
			Pattern(@",", TokenType.Comma),
			Pattern(@"\?", TokenType.ReturnValue),
			Pattern(@"(?:>=|<=|<>|==|[><=+\-*\/@~])", TokenType.Operator),
			Pattern(@""".*?(?<!\\)""", TokenType.String),
			Pattern(@"[\{(\[]", TokenType.OpeningBracket),
			Pattern(@"[\})\]]", TokenType.ClosingBracket),
		};
	}

	[Flags]
	public enum EmitOptions
	{
		None = 0,
		EmitWhitespace = 1,
		EmitComments = 2,
	}
}