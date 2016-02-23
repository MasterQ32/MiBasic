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

		public override void AssignType(CodeEnvironment environment)
		{
			this.Target.AssignType(environment);
			this.Value.AssignType(environment);

			if (this.Target.Type != this.Value.Type)
				throw new SemanticException($"Type mismatch: Cannot assign {this.Value.Type} to {this.Target.Type}");

			this.Type = this.Target.Type;
		}

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