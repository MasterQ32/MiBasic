namespace MiBasic.MiddleEnd
{
	public sealed class AssignmentExpression : Expression
	{
		public AssignmentExpression(Expression lhsexp, Expression rhsexp) :
			base(lhsexp.Type, true)
		{
			if (lhsexp.IsAssignable == false)
				throw new SemanticException("Assignment to inassignable expression!");
			if (lhsexp.Type != rhsexp.Type)
				throw new SemanticException($"Type mismatch: {rhsexp.Type} is not assignable to {lhsexp.Type}");
			this.Target = lhsexp;
			this.Value = rhsexp;
		}

		public Expression Target { get; private set; }

		public Expression Value { get; private set; }

		public override string ToString()
		{
			return
				"(" +
				(this.Target?.ToString() ?? "<null>") +
				" = " +
				(this.Value?.ToString() ?? "<null>") +
				")";
		}
	}
}