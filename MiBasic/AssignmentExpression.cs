namespace MiBasic
{
	public sealed class AssignmentExpression : Expression
	{
		public override bool IsAssignable => true;

		public AssignmentExpression(Expression lhsexp, Expression rhsexp)
		{
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