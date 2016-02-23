using System;

namespace MiBasic.MiddleEnd
{
	public sealed class UnaryOperatorExpression : Expression
	{
		public UnaryOperatorExpression(UnaryOperator @operator, Expression expression)
			: base(expression.Type)
		{
			this.Operator = @operator;
			this.Expression = expression;
		}

		public UnaryOperator Operator { get; private set; }
		public Expression Expression { get; private set; }
		
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
}