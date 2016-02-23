using MiBasic.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiBasic.Tools
{
	public static class DeclarationDumper
	{
		public static void DumpToStream(ModuleDeclaration decl, Stream stream)
		{
			using (var sw = new StreamWriter(stream, new UTF8Encoding(false)))
			{
				sw.WriteLine("digraph {");

				Write(sw, decl);

				sw.WriteLine("}");
			}
		}

		private static void Write(StreamWriter writer, ModuleDeclaration decl)
		{
			writer.WriteLine($"module [label=\"{decl.Name}\"];");
            foreach(var var in decl.GlobalVariables)
			{
				writer.WriteLine($"{decl.Name} -> {var.Name};");
				writer.WriteLine($"{var.Name} [color=blue, label=\"{var.Name} : {var.Type}\"];");
            }
			foreach(var type in decl.StructureTypes)
			{
				writer.WriteLine($"{decl.Name} -> {type.Name};");
				writer.WriteLine($"{type.Name} [color=red, label=\"{type.Name}\"];");

				foreach(var member in type.Members)
				{
					writer.WriteLine($"{type.Name} -> {member.Name};");
					writer.WriteLine($"{member.Name} [color=black, label=\"{member.Name} : {member.Type}\"];");
                }
            }
			foreach(var function in decl.Functions)
			{
				writer.WriteLine($"{decl.Name} -> {function.Name};");
				writer.WriteLine($"{function.Name} [color=yellow, label=\"{function.Name}() : {function.ReturnType}\"];");

				foreach (var param in function.Parameters)
				{
					writer.WriteLine($"{function.Name} -> {function.Name}_{param.Name};");
					writer.WriteLine($"{function.Name}_{param.Name} [color=cyan, label=\"{param.Name} : {param.Type}\"];");
				}

				foreach (var local in function.LocalVariables)
				{
					writer.WriteLine($"{function.Name} -> {function.Name}_{local.Name};");
					writer.WriteLine($"{function.Name}_{local.Name} [color=magenta, label=\"{local.Name} : {local.Type}\"];");
				}
			}
		}
	}
}
