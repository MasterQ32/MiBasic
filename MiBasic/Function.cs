using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiBasic
{
	public class Function : INamedObject
	{
		public string Name { get; set; }

		public BasicType ReturnType { get; set; } = BasicType.Void;

		public ParameterList Parameters { get; } = new ParameterList();

		public Block Code { get; set; } = new Block();

		public Container<LocalVariable> LocalVariables { get; } = new Container<LocalVariable>();

		public override string ToString()
		{
			return $"{this.Name}({ string.Join(", ", this.Parameters) }) : {this.ReturnType}";
        }
	}

	public sealed class ParameterList : IReadOnlyList<Parameter>
	{
		private readonly List<Parameter> list = new List<Parameter>();

		public void Add(Parameter param)
		{
			if (this.list.Any(p => p.Name.IsSemanticEqualTo(param.Name)))
				throw new InvalidOperationException("Parameter already exists.");
			param.Position = this.list.Count;
			this.list.Add(param);
		}

		public Parameter this[int index]
		{
			get
			{
				return list[index];
			}
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public IEnumerator<Parameter> GetEnumerator()
		{
			return ((IReadOnlyList<Parameter>)list).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IReadOnlyList<Parameter>)list).GetEnumerator();
		}
	}
}