namespace MiBasic.Parser
{
	public sealed class VariableDeclaration : INamedObject
	{
		public string Name { get; set; }
		public BasicTypeName Type { get; set; }
	}
}