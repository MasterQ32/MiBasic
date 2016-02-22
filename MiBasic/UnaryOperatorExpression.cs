using System;

namespace MiBasic
{
	public sealed class UnaryOperatorExpression : Expression
	{
		public override bool IsAssignable => true;

		public UnaryOperator Operator { get; private set; }
		public Expression Expression { get; private set; }

		public UnaryOperatorExpression(UnaryOperator unaryOperator, Expression expr)
		{
			this.Operator = unaryOperator;
			this.Expression = expr;
		}

		private string GetOperatorString()
		{
			switch(this.Operator)
			{
				case UnaryOperator.Invert: return "~";
				case UnaryOperator.Negate: return "-";
				default: return "??";
			}
		}

		public override string ToString()
		{
			return this.GetOperatorString() + "(" + (this.Expression?.ToString() ?? "<null>") + ")";
		}
	}

	public enum UnaryOperator
	{
		Negate,
		Invert,
	}
}