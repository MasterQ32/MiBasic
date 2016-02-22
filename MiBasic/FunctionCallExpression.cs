using System;
using System.Linq;

namespace MiBasic
{
	public sealed class FunctionCallExpression : Expression
	{
		public override bool IsAssignable => false;

		public FunctionCallExpression(string name, Expression[] parameters)
		{
			this.Name = name;
			this.Parameters = parameters;
		}

		public string Name { get; private set; }
		public Expression[] Parameters { get; private set; }

		public override string ToString()
		{
			return
				(this.Name ?? "<null>") +
				"(" +
				string.Join(
					", ", 
					this.Parameters.Select(p => p.ToString() ?? "<null>")) + 
				")";
		}
	}
}