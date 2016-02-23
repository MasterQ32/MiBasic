namespace MiBasic
{
	public sealed class CodeEnvironment
	{
		public IContainer<Function> Functions { get; set; }
		public IContainer<BasicType> Types { get; set; }
		public IContainer<Variable> Variables { get; set; }
	}
}