using System;

namespace MiBasic.MiddleEnd
{
	public abstract class Instruction
	{
		protected Instruction(bool isTopLevelAllowed)
		{
			this.IsAllowedOnTopLevel = isTopLevelAllowed;
		}

		public bool IsAllowedOnTopLevel { get; private set; }
	}

	public class ExpressionInstruction : Instruction
	{
		public ExpressionInstruction(Expression expression) 
			: base(expression is AssignmentExpression || expression is FunctionCallExpression)
		{
			this.Expression = expression;
		}

		public Expression Expression { get; private set; }

		public override string ToString() => this.Expression?.ToString() ?? "<null>";
	}
}