using System;

namespace MiBasic
{
	public abstract class Expression
	{
		public BasicType Type { get; set; }

		public abstract bool IsAssignable { get; }

		public virtual void AssignType(CodeEnvironment environment)
		{
			throw new SemanticException($"{this.GetType().Name} does not support any kind of type matching.");
		}
	}

	public class NumberExpression : Expression
	{
		public override bool IsAssignable => false;

		public string Literal { get; private set; }
		
		public NumberType NumberType { get; private set; }

		public static NumberExpression FromLiteral(string literal)
		{
			var expr = new NumberExpression();
			expr.Literal = literal;
			return expr;
		}
		
		public override void AssignType(CodeEnvironment environment)
		{
			switch(this.NumberType)
			{
				case NumberType.Integer:
				case NumberType.HexInteger:
					this.Type = environment.Types["INTEGER"];
					break;
				case NumberType.Real:
					this.Type = environment.Types["REAL"];
					break;
				default:
					throw new SemanticException($"Invalid number type: {this.NumberType}");
			}
		}

		public override string ToString() => this.Literal ?? "<null>";
	}

	public enum NumberType
	{
		Integer,
		HexInteger,
		Real,
	}

	public class VariableExpression : Expression
	{
		public override bool IsAssignable => true;

		public string VariableName { get; set; }

		public override string ToString() => this.VariableName ?? "<null>";

		public override void AssignType(CodeEnvironment environment)
		{
			var variable = environment.Variables[this.VariableName];
			if (variable == null)
				throw new SemanticException($"{this.VariableName} is not declared in this scope.");
			this.Type = variable.Type;
		}
	}
}