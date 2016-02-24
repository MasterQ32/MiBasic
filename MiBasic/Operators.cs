using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiBasic
{
	public static class Operators
	{
		public static UnaryOperator UnaryFromString(string text)
		{
			switch (text)
			{
				case "~": return UnaryOperator.Invert;
				case "-": return UnaryOperator.Negate;
				default:
					throw new InvalidOperationException($"{text} is not a valid unary operator.");
			}
		}

		public static BinaryOperator BinaryFromString(string text)
		{
			switch (text)
			{
				case "+": return BinaryOperator.Add;
				case "-": return BinaryOperator.Subtract;
				case "*": return BinaryOperator.Multiply;
				case "/": return BinaryOperator.Divide;
				case "%": return BinaryOperator.Modulo;
				case "=": return BinaryOperator.Assignment;
				default:
					throw new InvalidOperationException($"{text} is not a valid binary operator.");
			}
		}

		public static string ToString(BinaryOperator @operator)
		{
			switch (@operator)
			{
				case BinaryOperator.Add: return "+";
				case BinaryOperator.Assignment: return "=";
				case BinaryOperator.Divide: return "/";
				case BinaryOperator.Modulo: return "%";
				case BinaryOperator.Multiply: return "*";
				case BinaryOperator.Subtract: return "-";
				default: throw new InvalidOperationException($"Invalid operator: {@operator}.");
			}
		}

		public static string ToString(UnaryOperator @operator)
		{
			switch (@operator)
			{
				case UnaryOperator.Invert: return "~";
				case UnaryOperator.Negate: return "-";
				default: throw new InvalidOperationException($"Invalid operator: {@operator}.");
			}
		}
	}

	public enum UnaryOperator
	{
		Negate,
		Invert,
	}

	public enum BinaryOperator
	{
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulo,
		Assignment,
	}
}
