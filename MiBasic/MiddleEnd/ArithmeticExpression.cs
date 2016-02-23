using System;

namespace MiBasic.MiddleEnd
{
	public sealed class ArithmeticExpression : Expression
	{
		public ArithmeticExpression(
			Expression lhsexp, 
			Expression rhsexp,
			BinaryOperator op)
			: base(lhsexp.Type)
		{
			if (lhsexp.Type != rhsexp.Type)
				throw new SemanticException($"Type mismatch: {this.LeftHandSide.Type} != {this.RightHandSide.Type}");
			this.LeftHandSide = lhsexp;
			this.RightHandSide = rhsexp;
			this.Operator = op;
		}

		public Expression LeftHandSide { get; private set; }
		public BinaryOperator Operator { get; private set; }
		public Expression RightHandSide { get; private set; }
		
		private string GetOperatorString()
		{
			switch(this.Operator)
			{
				case BinaryOperator.Add: return "+";
				case BinaryOperator.Subtract: return "-";
				case BinaryOperator.Multiply: return "*";
				case BinaryOperator.Divide: return "/";
				case BinaryOperator.Modulo: return "%";
				default: return "??";
			}
		}

		public override string ToString()
		{
			return
				"(" + 
				(this.LeftHandSide?.ToString() ?? "<null>") +
				" " + this.GetOperatorString() + " " +
				(this.RightHandSide?.ToString() ?? "<null>") + 
				")";
		}
	}
}