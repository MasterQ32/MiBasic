using System;

namespace MiBasic
{
	public sealed class ArithmeticExpression : Expression
	{
		public override bool IsAssignable => false;

		public ArithmeticExpression(
			Expression lhsexp, 
			Expression rhsexp,
			BinaryOperator op)
		{
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