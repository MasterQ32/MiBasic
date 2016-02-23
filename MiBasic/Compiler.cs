using MiBasic.Lexer;
using MiBasic.Parser;
using MiBasic.Tools;
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
			var tokens = Tokenizer.Tokenize(Source);

			var moduleDeclaration = BasicParser.Parse(tokens);

			var globalContext = new CodeEnvironment();
			{ // global types
				globalContext.Types.Register(BasicType.Void);
				globalContext.Types.Register(new PrimitiveType() { Name = "Integer" });
				globalContext.Types.Register(new PrimitiveType() { Name = "Real" });
				globalContext.Types.Register(new PrimitiveType() { Name = "String" });
				globalContext.Types.Register(new PrimitiveType() { Name = "Bool" });
			}

			// extract all types
			foreach (var typeDecl in moduleDeclaration.StructureTypes)
			{
				var type = new StructureType();
				type.Name = typeDecl.Name;

				if (globalContext.Types[type.Name] != null)
					throw new SemanticException($"Duplicated type name: {type.Name}");
				globalContext.Types.Register(type);
			}

			// setup all extracted types 
			// this allows using circular dependencies and
			// also order-independant declarations
			foreach (var typeDecl in moduleDeclaration.StructureTypes)
			{
				var type = (StructureType)globalContext.Types[typeDecl.Name];

				foreach (var member in typeDecl.Members)
				{
					if (type[member.Name] != null)
						throw new SemanticException($"Duplicate member name: {member.Name}");
					type.Add(member.Name, globalContext.Types[member.Type.Name]);
				}
			}

			// gather all global variables
			foreach(var varDecl in moduleDeclaration.GlobalVariables)
			{
				globalContext.Variables.Register(new Variable()
				{
					Name = varDecl.Name,
					Type = globalContext.Types[varDecl.Type.Name],
				});
			}

			// gather all functions (without expression translation)
			foreach(var funDecl in moduleDeclaration.Functions)
			{
				if (globalContext.Functions[funDecl.Name] != null)
					throw new SemanticException($"Duplicate declaration of {funDecl.Name}");

				var function = new Function();
				function.Name = funDecl.Name;
				if (funDecl.ReturnType != null)
					function.ReturnType = globalContext.Types[funDecl.ReturnType.Name];
				else
					function.ReturnType = BasicType.Void;
			
				foreach(var local in funDecl.LocalVariables)
				{
					function.LocalVariables.Register(new LocalVariable()
					{
						Name = local.Name,
						Type = globalContext.Types[local.Type.Name],
					});
				}
				foreach(var param in funDecl.Parameters)
				{
					function.Parameters.Add(new Parameter()
					{
						Name = param.Name,
						Type = globalContext.Types[param.Type.Name],
					});
				}

				globalContext.Functions.Register(function);
			}

			/*
			// Sanitize code:
			foreach(var function in globalContext.Functions)
			{
				var variables = new Container<Variable>();
				foreach(var global in ast.Globals)
				{
					variables.Register(global);
				}
				foreach(var param in function.Parameters)
				{
					// Override globals with locals
					variables.Register(param, true);
				}
				foreach(var local in function.LocalVariables)
				{
					if (variables[local.Name]?.IsLocal == true)
						throw new SemanticException($"The variable name {local.Name} is already taken by a parameter.");
					variables.Register(local, true);
				}

				var context = new CodeEnvironment()
				{
					Variables = variables,
					Types = types,
					Functions = ast.Functions,
				};

				function.Code.Sanitize(context);
			}
			*/

			Console.WriteLine("done.");

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
