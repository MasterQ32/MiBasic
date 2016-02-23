namespace MiBasic
{
	public class Variable : INamedObject
	{
		public string Name { get; set; }

		public virtual bool IsLocal => false;

		public BasicType Type { get; set; }

		public override string ToString() => $"{this.Name} : {this.Type}";
    }

	public class LocalVariable : Variable
	{
		public override bool IsLocal => true;
	}

	public sealed class Parameter : LocalVariable
	{
		public int Position { get; set; }
	}
}