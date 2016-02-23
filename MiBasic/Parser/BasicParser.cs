using System;
using System.Collections.Generic;
using System.Linq;
using MiBasic.Lexer;

namespace MiBasic.Parser
{
	public class BasicParser : TokenReader
	{
		private BasicParser(Token[] tokens) : base(tokens)
		{

		}

		public static ModuleDeclaration Parse(Token[] tokens)
		{
			var parser = new BasicParser(tokens);
			return parser.Parse();
		}

		public static ExpressionTree ParseExpression(Token[] tokens)
		{
			var parser = new BasicParser(tokens);
			return parser.ParseExpression();
		}

		private ModuleDeclaration Parse()
		{
			var module = new ModuleDeclaration();

			while(this.EndOfTokens == false)
			{
				var keyword = this.ReadKeyword(Keyword.Variable, Keyword.Structure, Keyword.Function);
				switch (keyword)
				{
					case Keyword.Variable:
						{
							var variable = new VariableDeclaration();
							variable.Name = this.ReadIdentifier();
							this.ReadKeyword(Keyword.Is);
							variable.Type = this.ReadType();
							this.ReadDelimiter();
							module.GlobalVariables.Add(variable);
							break;
						}
					case Keyword.Structure:
						{
							var structureType = new StructureDeclaration();
							structureType.Name = this.ReadIdentifier();
							this.ReadKeyword(Keyword.Is);
							while (GetKeyword(this.PeekToken()) != Keyword.End)
							{
								var vardecl = new VariableDeclaration();
								vardecl.Name = this.ReadIdentifier();
								this.ReadKeyword(Keyword.Is);
								vardecl.Type = this.ReadType();
								this.ReadDelimiter();
								structureType.Members.Add(vardecl);
							}
							this.ReadKeyword(Keyword.End);
							module.StructureTypes.Add(structureType);
							break;
						}
					case Keyword.Function:
						{
							var function = new FunctionDeclaration();
							function.Name = this.ReadIdentifier();

							this.ReadCommaSeparatedList(() =>
							{
								var param = new VariableDeclaration();
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
										// here read all function specifications
										// WHERE, LOCAL, ...
										case Keyword.Local:
											var local = new VariableDeclaration();

											local.Name = this.ReadIdentifier();
											this.ReadKeyword(Keyword.Is);
											local.Type = this.ReadType();
											this.ReadDelimiter();

											function.LocalVariables.Add(local);
											break;
									}
								} while (specifier != Keyword.Implementation);

								this.ReadKeyword(Keyword.Is);
								function.Code = this.ReadBlock();
							}

							module.Functions.Add(function);
							break;
						}
				}
			}

			return module;
		}

		private int FindOperatorSplit(string optext)
		{
			int depth = 0;
			for (int i = 0; i < this.Length; i++)
			{
				var token = this.Tokens[i];
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
				var token = this.Tokens[i];
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

		private ExpressionTree ParseExpression()
		{
			if (this.Length == 0)
			{
				throw new InvalidOperationException("Invalid expression");
			}
			if (this.Length == 1)
			{
				// Must be a number or a variable access here...
				var token = this.Tokens[0];
				if (token.Type == TokenType.Number)
				{
					return new ExpressionTree()
					{
						AdditionalInformation = token.Text,
						Type = ExpressionType.NumberLiteral,
					};
				}
				else if (token.Type == TokenType.Identifier)
				{
					return new ExpressionTree()
					{
						AdditionalInformation = token.Text,
						Type = ExpressionType.IdentifierLiteral,
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

			if (this.Tokens[0].Type == TokenType.Operator)
			{
				// Must be unary operator
				if (unaryOperators.ContainsKey(this.Tokens[0].Text) == false)
					throw new ParserException(this.Tokens[0], "Invalid unary operator!");
				var expr = BasicParser.ParseExpression(this.Tokens.Skip(1).ToArray());
				return new ExpressionTree()
				{
					Type = ExpressionType.UnaryOperation,
					AdditionalInformation = this.Tokens[0].Text,
					Children = { expr }
				};
			}

			foreach (var op in binaryOperators)
			{
				int position;
				if ((position = FindOperatorSplit(op.Item1)) != -1)
				{
					var lhs = this.Tokens.Take(position).ToArray();
					var rhs = this.Tokens.Skip(position + 1).ToArray();

					var lhsexp = BasicParser.ParseExpression(lhs);
					var rhsexp = BasicParser.ParseExpression(rhs);

					return new ExpressionTree()
					{
						Type = ExpressionType.BinaryOperation,
						AdditionalInformation = op.Item1,
						Children = { lhsexp, rhsexp }
					};
				}
			}

			// Bracket group
			if (this.Tokens[0].Type == TokenType.OpeningBracket)
			{
				if (this.Tokens[this.Tokens.Count - 1].Type != TokenType.ClosingBracket)
					throw new ParserException(this.Tokens[0], "Missing closing bracket!");
				return BasicParser.ParseExpression(this.Tokens.Skip(1).Take(this.Tokens.Count - 2).ToArray());
			}

			if (this.Tokens[0].Type == TokenType.Identifier)
			{
				// first part is identifier, so we must have a function (variable would have length 1)
				if (this.Tokens[1].Type != TokenType.OpeningBracket)
					throw new ParserException(this.Tokens[1], "Expected opening bracket!");
				if (this.Tokens[this.Tokens.Count - 1].Type != TokenType.ClosingBracket)
					throw new ParserException(this.Tokens[0], "Missing closing bracket!");

				// now split:
				var splits = new Queue<int>(this.FindCommaSplits(2, this.Tokens.Count - 1).Select(s => s - 2));
				var paramlist = this.Tokens.Skip(2).Take(this.Tokens.Count - 3).ToArray();

				var parameters = paramlist
					.SplitAt(splits)
					.Select(list => BasicParser.ParseExpression(list.ToArray()))
					.ToList();

				return new ExpressionTree()
				{
					AdditionalInformation = this.Tokens[0].Text,
					Type = ExpressionType.FunctionCall,
					Children = parameters,
				};
			}


			throw new ParserException(this.Tokens[0], "Invalid expression.");
		}

		private List<InstructionDeclaration> ReadBlock(Keyword delimiter = Keyword.End)
		{
			var block = new List<InstructionDeclaration>();
			while (this.CheckForKeyword(delimiter) == false)
			{
				var instruction = this.ReadInstruction();
				block.Add(instruction);
			}
			this.ReadKeyword(delimiter);
			return block;
		}

		private InstructionDeclaration ReadInstruction()
		{
			var token = this.PeekToken();
			var keyword = GetKeyword(token);
			switch (keyword)
			{
				// Insert control blocks here....
				default:
					{
						var expression = this.ReadExpression((t) => t.Type == TokenType.Delimiter);
						return new ExpressionInstructionDeclaration()
						{
							Expression = expression
						};
					}
			}
		}

		private ExpressionTree ReadExpression(Func<Token, bool> isDelimiter)
		{
			var tokens = new List<Token>();
			for (int length = 0; isDelimiter(this.Tokens[this.Position + length]) == false; length++)
			{
				tokens.Add(this.Tokens[this.Position + length]);
			}
			this.Advance(tokens.Count + 1);
			return BasicParser.ParseExpression(tokens.ToArray());
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