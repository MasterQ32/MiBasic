using System;
using System.Linq;

namespace MiBasic
{
	public sealed class FunctionCallExpression : Expression
	{
		public override bool IsAssignable => false;

		public FunctionCallExpression(string name, Expression[] arguments)
		{
			this.Name = name;
			this.Arguments = arguments;
		}

		public string Name { get; private set; }
		public Expression[] Arguments { get; private set; }

		public override void AssignType(CodeEnvironment environment)
		{
			var func = environment.Functions[this.Name];
			if (func == null)
				throw new SemanticException($"Function {this.Name} not found.");

			if (this.Arguments.Length != func.Parameters.Count)
				throw new SemanticException("Argument count mismatch!");

			for (int i = 0; i < this.Arguments.Length; i++)
			{
				this.Arguments[i].AssignType(environment);
				if (this.Arguments[i].Type != func.Parameters[i].Type)
					throw new SemanticException("Argument type mismatch!");
			}

			this.Type = func.ReturnType;
		}

		public override string ToString()
		{
			return
				(this.Name ?? "<null>") +
				"(" +
				string.Join(
					", ", 
					this.Arguments.Select(p => p.ToString() ?? "<null>")) + 
				")";
		}
	}
}