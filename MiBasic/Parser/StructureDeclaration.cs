using System.Collections.Generic;

namespace MiBasic.Parser
{
	public class StructureDeclaration : INamedObject
	{
		public string Name { get; set; }

		public IList<VariableDeclaration> Members { get; } = new List<VariableDeclaration>();
	}
}