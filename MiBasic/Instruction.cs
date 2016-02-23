using System;

namespace MiBasic
{
	public abstract class Instruction
	{
		public virtual void Sanitize(CodeEnvironment env)
		{
			
		}
	}

	public class ExpressionInstruction : Instruction
	{
		public Expression Expression { get; set; }

		public override string ToString() => this.Expression?.ToString() ?? "<null>";

		public override void Sanitize(CodeEnvironment env)
		{
			this.Expression.AssignType(env);
		}
	}
}