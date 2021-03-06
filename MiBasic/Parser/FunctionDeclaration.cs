﻿using System;
using System.Collections.Generic;

namespace MiBasic.Parser
{
	public class FunctionDeclaration : INamedObject
	{
		public string Name { get; set; }

		public IList<VariableDeclaration> Parameters { get; } = new List<VariableDeclaration>();

		public IList<VariableDeclaration> LocalVariables { get; } = new List<VariableDeclaration>();

		public BasicTypeName ReturnType { get; set; }

		public IList<InstructionDeclaration> Code { get; set; } = null;
	}

	public abstract class InstructionDeclaration
	{
	}

	public sealed class ExpressionInstructionDeclaration : InstructionDeclaration
	{
		public ExpressionTree Expression { get; set; }
	}
}