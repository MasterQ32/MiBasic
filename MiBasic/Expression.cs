using System;

namespace MiBasic
{
	public abstract class Expression
	{
		public BasicType Type { get; set; }

		public abstract bool IsAssignable { get; }
	}

	public class NumberExpression : Expression
	{
		public override bool IsAssignable => false;

		public string Literal { get; private set; }

		public static NumberExpression FromLiteral(Container<BasicType> types, string literal)
		{
			var expr = new NumberExpression();
			expr.Literal = literal;
			expr.Type = types["INTEGER"];
			return expr;
		}
		public override string ToString() => this.Literal ?? "<null>";
	}

	public class VariableExpression : Expression
	{
		public override bool IsAssignable => true;

		public string VariableName { get; set; }

		public override string ToString() => this.VariableName ?? "<null>";
	}
}