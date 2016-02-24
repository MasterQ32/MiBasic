using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiBasic.MiddleEnd;
using System.IO;

namespace MiBasic.BackEnd.C
{
	public sealed class CodeGenerator
	{
		public static void Generate(TextWriter writer, Module module)
		{
			writer.WriteLine("typedef int Integer;");
			writer.WriteLine("typedef float Real;");
			writer.WriteLine("typedef int Bool;");

			writer.WriteLine();
			writer.WriteLine("// Structure types");
			foreach (var type in module.StructureTypes)
				writer.WriteLine("typedef struct {0} {0};", type.Name);

			foreach (var type in module.StructureTypes)
			{
				writer.WriteLine("struct {0} {{", type.Name);

				foreach (var member in type)
				{
					writer.WriteLine("\t{0} {1};", member.Type, member.Name);
				}

				writer.WriteLine("};");
			}
			writer.WriteLine();

			writer.WriteLine("// Global variables");
			foreach (var var in module.Globals)
			{
				writer.WriteLine("{0} {1};", var.Type, var.Name);
			}
			writer.WriteLine();

			writer.WriteLine("// Function prototypes");
			foreach (var fun in module.Functions)
			{
				writer.WriteLine(
					"{0} {1}({2});",
					fun.ReturnType,
					fun.Name,
					string.Join(", ", fun.Parameters.Select(p => $"{p.Type} {p.Name}")));
			}
			writer.WriteLine();

			writer.WriteLine("// Function implementations");
			foreach (var fun in module.Functions.Where(f => f.IsImplemented))
			{
				writer.WriteLine(
					"{0} {1}({2}) {{",
					fun.ReturnType,
					fun.Name,
					string.Join(", ", fun.Parameters.Select(p => $"{p.Type} {p.Name}")));

				GenerateBlock(writer, fun.Code);

				writer.WriteLine("}");
			}
			writer.WriteLine();
		}

		private static void GenerateBlock(TextWriter writer, Block code)
		{
			foreach (var instr in code)
			{
				ExpressionInstruction expri;
				if (OfType(instr, out expri))
				{
					GenerateExpression(writer, expri.Expression);
					writer.WriteLine(";");
				}
			}
		}

		private static void GenerateExpression(TextWriter writer, Expression expression)
		{
			expression.Dispatch<ArithmeticExpression>((e) =>
			{
				writer.Write("(");
				GenerateExpression(writer, e.LeftHandSide);
				writer.Write(" {0} ", Operators.ToString(e.Operator));
				GenerateExpression(writer, e.RightHandSide);
				writer.Write(")");
			});
			expression.Dispatch<AssignmentExpression>((e) =>
			{
				writer.Write("(");
				GenerateExpression(writer, e.Target);
				writer.Write(" = ");
				GenerateExpression(writer, e.Value);
				writer.Write(")");
			});
			expression.Dispatch<UnaryOperatorExpression>((e) =>
			{
				writer.Write(Operators.ToString(e.Operator));
				writer.Write("(");
				GenerateExpression(writer, e.Expression);
				writer.Write(")");
			});
			expression.Dispatch<NumberExpression>((e) =>
			{
				writer.Write(e.Literal);
			});
			expression.Dispatch<VariableExpression>((e) =>
			{
				writer.Write(e.Variable.Name);
			});
			expression.Dispatch<FunctionCallExpression>((e) =>
			{
				writer.Write(e.Function.Name);
				writer.Write("(");

				for (int i = 0; i < e.Arguments.Length; i++)
				{
					if (i > 0)
						writer.Write(", ");
					GenerateExpression(writer, e.Arguments[i]);
				}

				writer.Write(")");
			});
		}

		private static bool OfType<T1, T2>(T1 value, out T2 result)
			where T1 : class
			where T2 : class
		{
			result = value as T2;
			return result != null;
		}
	}

	static class Extensions
	{
		public static void Dispatch<T>(this object o, Action<T> act)
			where T : class
		{
			T value = o as T;
			if (value != null)
				act(value);
		}
	}
}
