namespace MiBasic.MiddleEnd
{
	public sealed class Module
	{
		public Container<Variable> Globals { get; } = new Container<Variable>();

		public Container<StructureType> StructureTypes { get; } = new Container<StructureType>();

		public Container<Function> Functions { get; } = new Container<Function>();
	}
}