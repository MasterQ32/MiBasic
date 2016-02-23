using System.Collections.Generic;

namespace MiBasic.Parser
{
	public sealed class ModuleDeclaration : INamedObject
	{
		public string Name { get; set; } = "module";

		public IList<VariableDeclaration> GlobalVariables { get; } = new List<VariableDeclaration>();

		public IList<FunctionDeclaration> Functions { get; } = new List<FunctionDeclaration>();

		public IList<StructureDeclaration> StructureTypes { get; } = new List<StructureDeclaration>();
	}
}