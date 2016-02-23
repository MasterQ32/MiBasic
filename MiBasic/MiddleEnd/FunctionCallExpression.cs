using System;
using System.Linq;

namespace MiBasic.MiddleEnd
{
	public sealed class FunctionCallExpression : Expression
	{
		public FunctionCallExpression(Function function, Expression[] arguments)
			 : base(function.ReturnType)
		{
			this.Function = function;
			this.Arguments = arguments;

			if (this.Arguments.Length != function.Parameters.Count)
				throw new SemanticException("Argument count mismatch!");

			for (int i = 0; i < this.Arguments.Length; i++)
			{
				if (this.Arguments[i].Type != function.Parameters[i].Type)
					throw new SemanticException("Argument type mismatch!");
			}
		}
		
		public Expression[] Arguments { get; private set; }

		public Function Function { get; private set; }

		public override string ToString()
		{
			return
				(this.Function.Name ?? "<null>") +
				"(" +
				string.Join(
					", ", 
					this.Arguments.Select(p => p.ToString() ?? "<null>")) + 
				")";
		}
	}
}