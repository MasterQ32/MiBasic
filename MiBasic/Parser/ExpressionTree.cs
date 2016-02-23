using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiBasic.Parser
{
	public sealed class ExpressionTree
	{
		/// <summary>
		/// The additional information for the specified expression type.
		/// Can be operator type, function name, ...
		/// </summary>
		public string AdditionalInformation { get; set; }

		public ExpressionType Type { get; set; } = ExpressionType.Invalid;

		public IList<ExpressionTree> Children { get; set; } = new List<ExpressionTree>();
	}

	public enum ExpressionType
	{
		Invalid,
		BinaryOperation,
		UnaryOperation,
		FunctionCall,
		NumberLiteral,
		IdentifierLiteral,
	}
}
