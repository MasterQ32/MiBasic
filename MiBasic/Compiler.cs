using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MiBasic
{
	class Compiler
	{
		public static bool IsCaseSensitive = false;

		static readonly string Source = GetResource("MiBasic.Source.bas");

		static void Main(string[] args)
		{
			var types = new Container<BasicType>();
			{
				types.Register(BasicType.Void);
				types.Register(new PrimitiveType() { Name = "Integer" });
				types.Register(new PrimitiveType() { Name = "Real" });
				types.Register(new PrimitiveType() { Name = "String" });
				types.Register(new PrimitiveType() { Name = "Bool" });
			}

			var tokens = Tokenizer.Tokenize(Source);

			/*
			foreach (var token in tokens)
			{
				Console.WriteLine(token);
			}
			// */

			var ast = Parser.Parse(types, tokens);

			Console.WriteLine("Module dump:");
			foreach(var variable in ast.Globals)
			{
				Console.WriteLine("\t{0} : {1}", variable.Name, variable.Type);
			}
			foreach (var type in ast.StructureTypes)
			{
				Console.WriteLine("\t{0} [", type.Name);
				foreach(var member in type)
				{
					Console.WriteLine("\t\t{0} : {1}", member.Name, member.Type);
				}
				Console.WriteLine("\t]");
			}

			foreach(var function in ast.Functions)
			{
				Console.WriteLine(
					"{0} {1}({2})",
					function.ReturnType,
					function.Name,
					string.Join(", ", function.Parameters.Select(p => $"{p.Type} {p.Name}")));
				foreach (var instr in function.Code)
				{
					Console.WriteLine("\t{0}", instr.ToString());
				}
            }

			Console.ReadLine();
		}

		private static string GetResource(string name)
		{
			using (var resourceStream = typeof(Compiler).Assembly.GetManifestResourceStream(name))
			{
				using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}

	static class Extensions
	{
		public static bool IsSemanticEqualTo(this string lhs, string rhs)
		{
			if (lhs == null)
				return rhs == null;
			if (rhs == null)
				return false;
			if (Compiler.IsCaseSensitive)
				return lhs == rhs;
			return lhs.ToLowerInvariant() == rhs.ToLowerInvariant();
		}



		public static bool Has(this IEnumerable<Token> list, TokenType type)
		{
			return list.Any(t => t.Type == type);
		}

		/// <summary>
		/// Spaltet eine Sequenz von Elementen, an Elementen mit bestimmten Merkmalen, in mehrere Teilsequenzen auf.
		/// </summary>
		/// <typeparam name="T">Der Typ der Elemente.</typeparam>
		/// <param name="source">Die zu spaltende Sequenz.</param>
		/// <param name="splitter">Die Funktion, mit der die Elemente ermittelt werden, an denen die Sequenz aufgespalten werden soll.</param>
		/// <returns>Eine Sequenz mit Teilsequenzen von <paramref name="source"/>, aufgespalten an Elementen, die mit <paramref name="splitter"/> ermittelt wurden.</returns>
		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, Func<T, bool> splitter)
		{
			using (var enu = source.GetEnumerator())
			{
				while (enu.MoveNext())
				{
					yield return GetInnerSequence(enu, splitter).ToList();
				}
			}
		}

		private static IEnumerable<T> GetInnerSequence<T>(IEnumerator<T> enu, Func<T, bool> splitter)
		{
			do
			{
				if (splitter(enu.Current))
					yield break;
				yield return enu.Current;
			} while (enu.MoveNext());
		}

		private class IntRef
		{
			public int value;
		}

		public static IEnumerable<IEnumerable<T>> SplitAt<T>(this IEnumerable<T> source, IEnumerable<int> splits)
		{
			var splitter = new Queue<int>(splits.OrderBy(k => k));
			var index = new IntRef() { value = 0 };
			using (var enu = source.GetEnumerator())
			{
				while (enu.MoveNext())
				{
					yield return GetInnerSequence(enu, index, splitter).ToList();
				}
			}
		}

		private static IEnumerable<T> GetInnerSequence<T>(IEnumerator<T> enu, IntRef index, Queue<int> splitter)
		{
			do
			{
				if ((splitter.Count > 0) && (splitter.Peek() == index.value))
				{
					splitter.Dequeue();
					yield break;
				}
				index.value++;
				yield return enu.Current;
			} while (enu.MoveNext());
		}
	}
}
