using System;

namespace MiBasic.MiddleEnd
{
	public abstract class Expression
	{
		public BasicType Type { get; private set; }

		public bool IsAssignable { get; private set; }

		protected Expression(BasicType type)
			: this(type, false)
		{
		}

		protected Expression(BasicType type, bool isAssignable)
		{
			this.Type = type;
			this.IsAssignable = isAssignable;
		}

		public virtual void AssignType(CodeEnvironment environment)
		{
			throw new SemanticException($"{this.GetType().Name} does not support any kind of type matching.");
		}
	}

	public class NumberExpression : Expression
	{
		public NumberExpression(BasicType type, NumberType numberType, string literal) : 
			base(type)
		{
			this.NumberType = numberType;
			this.Literal = literal;
		}

		public string Literal { get; private set; }
		
		public NumberType NumberType { get; private set; }
		
		public override string ToString() => this.Literal;
	}

	public enum NumberType
	{
		Integer,
		HexInteger,
		Real,
	}

	public class VariableExpression : Expression
	{
		public VariableExpression(Variable variable) : 
			base(variable.Type, true)
		{
			this.Variable = variable;
		}

		public Variable Variable { get; set; }

		public override string ToString() => this.Variable.Name;
	}
}