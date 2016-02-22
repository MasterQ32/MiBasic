namespace MiBasic
{
	public abstract class Instruction
	{

	}

	public class ExpressionInstruction : Instruction
	{
		public Expression Expression { get; set; }

		public override string ToString() => this.Expression?.ToString() ?? "<null>";
	}
}