namespace MiBasic
{
	public sealed class CodeEnvironment
	{
		public CodeEnvironment Derive()
		{
			var env = new CodeEnvironment();

			env.Functions = new Container<Function>(this.Functions);
			env.Types = new Container<BasicType>(this.Types);
			env.Variables = new Container<Variable>(this.Variables);

			return env;
		}

		public Container<Function> Functions { get; set; } = new Container<Function>();

		public Container<BasicType> Types { get; set; } = new Container<BasicType>();

		public Container<Variable> Variables { get; set; } = new Container<Variable>();
	}
}