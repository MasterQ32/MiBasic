namespace MiBasic
{
	public sealed class Variable : INamedObject
	{
		public string Name { get; set; }

		public bool IsLocal { get; set; }

		public BasicType Type { get; set; }
	}
}