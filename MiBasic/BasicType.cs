using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiBasic
{
	public abstract class BasicType : INamedObject
	{
		public string Name { get; set; }

		public bool IsPrimitive => this is PrimitiveType;
		public bool IsStructure => this is StructureType;
		public bool IsArray => this is PrimitiveType;

		public static BasicType Void { get; } = new VoidType();

		public override string ToString() => this.Name;

		private class VoidType : BasicType
		{
			public VoidType()
			{
				this.Name = "VOID";
			}
		}
	}

	public sealed class PrimitiveType : BasicType
	{

	}

	public sealed class StructureType : BasicType, IReadOnlyList<StructureMember>
	{
		private readonly List<StructureMember> members = new List<StructureMember>();

		public void Add(string name, BasicType type)
		{
			if (this.members.Any(i => i.Name == name))
				throw new InvalidOperationException("A member with the name already exists.");
			this.members.Add(new StructureMember(name, type));
		}

		public StructureMember this[string name] => this.members.FirstOrDefault(i => i.Name == name);

		public StructureMember this[int index] => members[index];

		public int Count => members.Count;

		public IEnumerator<StructureMember> GetEnumerator()
		{
			return this.members.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}

	public sealed class StructureMember
	{
		public StructureMember(string name, BasicType type)
		{
			this.Name = name;
			this.Type = type;
		}

		public string Name { get; private set; }

		public BasicType Type { get; private set; }
	}

	public sealed class ArrayType : BasicType
	{
		public BasicType UnderlyingType { get; set; }
	}
}