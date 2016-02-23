using System;
using System.Collections.Generic;
using System.Linq;

namespace MiBasic
{
	public class Parser
	{
		private readonly Container<BasicType> types;
		private readonly Token[] tokens;
		private int cursor = 0;

		private Parser(Container<BasicType> types, Token[] tokens)
		{
			this.types = types;
			this.tokens = tokens;
		}

		public static Module Parse(Container<BasicType> types, Token[] tokens)
		{
			var parser = new Parser(types, tokens);
			return parser.Parse();
		}

		public static Expression ParseExpression(Container<BasicType> types, Token[] tokens)
		{
			var parser = new Parser(types, tokens);
			return parser.ParseExpression();
		}

		private Module Parse()
		{
			var module = new Module();

			for (this.cursor = 0; this.cursor < this.tokens.Length;)
			{
				var keyword = this.ReadKeyword(Keyword.Variable, Keyword.Structure, Keyword.Function);
				switch (keyword)
				{
					case Keyword.Variable:
						{
							var variable = new Variable();
							variable.Name = this.ReadIdentifier();
							this.ReadKeyword(Keyword.Is);
							variable.Type = this.ReadType();
							this.ReadDelimiter();
							module.Globals.Register(variable);
							break;
						}
					case Keyword.Structure:
						{
							var structureType = new StructureType();
							structureType.Name = this.ReadIdentifier();
							this.ReadKeyword(Keyword.Is);
							while (GetKeyword(this.PeekToken()) != Keyword.End)
							{
								var name = this.ReadIdentifier();
								this.ReadKeyword(Keyword.Is);
								var type = this.ReadType();
								this.ReadDelimiter();
								structureType.Add(name, type);
							}
							this.ReadKeyword(Keyword.End);
							module.StructureTypes.Register(structureType);
							break;
						}
					case Keyword.Function:
						{
							var function = new Function();
							function.Name = this.ReadIdentifier();

							this.ReadCommaSeparatedList(() =>
							{
								var param = new Parameter();
								param.Name = this.ReadIdentifier();
								this.ReadKeyword(Keyword.Is);
								param.Type = this.ReadType();
								function.Parameters.Add(param);
							});

							if (this.CheckForKeyword(Keyword.Is))
							{
								this.ReadKeyword(Keyword.Is);
								function.ReturnType = this.ReadType();
							}

							// here read all function specifications
							// WHERE, LOCAL, ...

							if (this.PeekToken().Type == TokenType.Delimiter)
							{
								// Function Prototype
								this.ReadDelimiter();
							}
							else
							{
								Keyword specifier;
								do
								{
									specifier = this.ReadKeyword(Keyword.Implementation, Keyword.Local);
									switch (specifier)
									{
										case Keyword.Local:
											var local = new LocalVariable();

											local.Name = this.ReadIdentifier();
											this.ReadKeyword(Keyword.Is);
											local.Type = this.ReadType();
											this.ReadDelimiter();

											function.LocalVariables.Register(local);
											break;
									}
								} while (specifier != Keyword.Implementation);

								this.ReadKeyword(Keyword.Is);
								function.Code = this.ReadBlock();
							}

							module.Functions.Register(function);
							break;
						}
				}
			}

			return module;
		}

		private int FindOperatorSplit(string optext)
		{
			int depth = 0;
			for (int i = 0; i < this.tokens.Length; i++)
			{
				var token = this.tokens[i];
				if (depth > 0)
				{
					switch (token.Type)
					{
						case TokenType.OpeningBracket:
							depth++;
							break;
						case TokenType.ClosingBracket:
							depth--;
							break;
						default:
							break;
					}
				}
				else
				{
					if (token.Type == TokenType.OpeningBracket)
						depth++;
					else if (token.Type == TokenType.Operator && token.Text == optext)
						return i;
				}
			}
			return -1;
		}

		private IEnumerable<int> FindCommaSplits(int start, int end)
		{
			int depth = 0;
			for (int i = start; i < end; i++)
			{
				var token = this.tokens[i];
				if (depth > 0)
				{
					switch (token.Type)
					{
						case TokenType.OpeningBracket:
							depth++;
							break;
						case TokenType.ClosingBracket:
							depth--;
							break;
						default:
							break;
					}
				}
				else
				{
					if (token.Type == TokenType.OpeningBracket)
						depth++;
					else if (token.Type == TokenType.Comma)
						yield return i;
				}
			}
		}

		private Expression ParseExpression()
		{
			if (this.tokens.Length == 0)
			{
				throw new InvalidOperationException("Invalid expression");
			}
			if (this.tokens.Length == 1)
			{
				// Must be a number or a variable access here...
				var token = this.tokens[0];
				if (token.Type == TokenType.Number)
				{
					return NumberExpression.FromLiteral(token.Text);
				}
				else if (token.Type == TokenType.Identifier)
				{
					return new VariableExpression()
					{
						VariableName = token.Text,
					};
				}
				else
				{
					throw new ParserException(token, "Expected a number or variable.");
				}
			}

			// parsing precedence:
			// 1. functions
			// 2. brackets
			// 3. unary operators
			// 4. binary operators

			var binaryOperators = new List<Tuple<string, BinaryOperator>>()
			{
				Tuple.Create("=", BinaryOperator.Assignment),

				Tuple.Create("+", BinaryOperator.Add),
				Tuple.Create("-", BinaryOperator.Subtract),

				Tuple.Create("*", BinaryOperator.Multiply),
				Tuple.Create("/", BinaryOperator.Divide),

				Tuple.Create("%", BinaryOperator.Modulo),
			};

			var unaryOperators = new Dictionary<string, UnaryOperator>()
			{
				{ "-", UnaryOperator.Negate},
				{ "~", UnaryOperator.Invert},
			};

			if (this.tokens[0].Type == TokenType.Operator)
			{
				// Must be unary operator
				if (unaryOperators.ContainsKey(this.tokens[0].Text) == false)
					throw new ParserException(this.tokens[0], "Invalid unary operator!");
				var expr = Parser.ParseExpression(this.types, this.tokens.Skip(1).ToArray());
				return new UnaryOperatorExpression(
					unaryOperators[this.tokens[0].Text],
					expr);
			}

			foreach (var op in binaryOperators)
			{
				int position;
				if ((position = FindOperatorSplit(op.Item1)) != -1)
				{
					var lhs = this.tokens.Take(position).ToArray();
					var rhs = this.tokens.Skip(position + 1).ToArray();

					var lhsexp = Parser.ParseExpression(this.types, lhs);
					var rhsexp = Parser.ParseExpression(this.types, rhs);

					if (op.Item2 == BinaryOperator.Assignment)
						return new AssignmentExpression(lhsexp, rhsexp);
					else
						return new ArithmeticExpression(lhsexp, rhsexp, op.Item2);
				}
			}

			// Bracket group
			if (this.tokens[0].Type == TokenType.OpeningBracket)
			{
				if (this.tokens[this.tokens.Length - 1].Type != TokenType.ClosingBracket)
					throw new ParserException(this.tokens[0], "Missing closing bracket!");
				return Parser.ParseExpression(this.types, this.tokens.Skip(1).Take(this.tokens.Length - 2).ToArray());
			}

			if (this.tokens[0].Type == TokenType.Identifier)
			{
				// first part is identifier, so we must have a function (variable would have length 1)
				if (this.tokens[1].Type != TokenType.OpeningBracket)
					throw new ParserException(this.tokens[1], "Expected opening bracket!");
				if (this.tokens[this.tokens.Length - 1].Type != TokenType.ClosingBracket)
					throw new ParserException(this.tokens[0], "Missing closing bracket!");

				// now split:
				var splits = new Queue<int>(this.FindCommaSplits(2, this.tokens.Length - 1).Select(s => s - 2));
				var paramlist = this.tokens.Skip(2).Take(this.tokens.Length - 3).ToArray();

				var parameters = paramlist
					.SplitAt(splits)
					.Select(list => Parser.ParseExpression(
						this.types,
						list.ToArray()))
					.ToArray();

				return new FunctionCallExpression(this.tokens[0].Text, parameters);
			}


			throw new ParserException(this.tokens[0], "Invalid expression.");
		}

		private Block ReadBlock(Keyword delimiter = Keyword.End)
		{
			var block = new Block();
			while (this.CheckForKeyword(delimiter) == false)
			{
				var instruction = this.ReadInstruction();
				block.Add(instruction);
			}
			this.ReadKeyword(delimiter);
			return block;
		}

		private Instruction ReadInstruction()
		{
			var token = this.PeekToken();
			var keyword = GetKeyword(token);
			switch (keyword)
			{
				// Insert control blocks here....
				default:
					{
						var expression = this.ReadExpression((t) => t.Type == TokenType.Delimiter);
						return new ExpressionInstruction()
						{
							Expression = expression
						};
					}
			}
		}

		private Expression ReadExpression(Func<Token, bool> isDelimiter)
		{
			var tokens = new List<Token>();
			for (int length = 0; isDelimiter(this.tokens[this.cursor + length]) == false; length++)
			{
				tokens.Add(this.tokens[this.cursor + length]);
			}
			this.cursor += tokens.Count + 1;
			return Parser.ParseExpression(this.types, tokens.ToArray());
		}

		private Token PeekToken()
		{
			return this.tokens[this.cursor];
		}

		private bool CheckForKeyword(Keyword keyword)
		{
			var token = this.PeekToken();
			return GetKeyword(token) == keyword;
		}

		private void ReadCommaSeparatedList(Action readItem)
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

		private void ReadSpecificToken(TokenType type)
		{
			var token = this.tokens[this.cursor++];
			if (token.Type != type)
				throw new ParserException(token, $"Expected {type}.");
		}

		private void ReadOpeningBracket() => ReadSpecificToken(TokenType.OpeningBracket);

		private void ReadClosingBracket() => ReadSpecificToken(TokenType.ClosingBracket);

		private void ReadDelimiter() => ReadSpecificToken(TokenType.Delimiter);

		private void ReadComma() => ReadSpecificToken(TokenType.Comma);

		private string ReadIdentifier()
		{
			var token = this.tokens[this.cursor++];
			if (token.Type != TokenType.Identifier)
				throw new ParserException(token, "Expected an identifier.");
			return token.Text;
		}

		private BasicType ReadType()
		{
			var token = this.tokens[this.cursor++];
			if (token.Type != TokenType.Identifier)
				throw new ParserException(token, "Expected a type name.");
			var type = this.types[token.Text];
			if (type == null)
				throw new ParserException(token, $"{token.Text} is not a type.");
			return type;
		}

		private Keyword ReadKeyword()
		{
			var keyword = GetKeyword(this.tokens[this.cursor]);
			if (keyword == Keyword.Invalid)
				throw new ParserException(this.tokens[this.cursor], "Expected a keyword.");
			this.cursor++;
			return keyword;
		}

		private Keyword ReadKeyword(params Keyword[] options)
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

	public enum Keyword
	{
		Invalid,

		Variable,
		Is,
		Structure,
		End,
		Function,
		Implementation,
		Local,
	}
}