using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiBasic.MiddleEnd
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

	/// <summary>
	/// A user defined structure type. Can be considered an ordered set of variables packed together.
	/// </summary>
	public sealed class StructureType : BasicType, IReadOnlyList<StructureMember>
	{
		private readonly List<StructureMember> members = new List<StructureMember>();

		/// <summary>
		/// Adds a new structure member to this type.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public void Add(string name, BasicType type)
		{
			if (this.members.Any(i => i.Name == name))
				throw new InvalidOperationException("A member with the name already exists.");
			this.members.Add(new StructureMember(name, type));
		}

		/// <summary>
		/// Gets a structure member by name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public StructureMember this[string name] => this.members.FirstOrDefault(i => i.Name == name);

		/// <summary>
		/// Gets a structure member by index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public StructureMember this[int index] => members[index];

		/// <summary>
		/// Gets the amount of structure members in this type.
		/// </summary>
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

	/// <summary>
	/// A member of a structure.
	/// </summary>
	public sealed class StructureMember
	{
		public StructureMember(string name, BasicType type)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (type == null) throw new ArgumentNullException(nameof(type));

			this.Name = name;
			this.Type = type;
		}

		public string Name { get; private set; }

		public BasicType Type { get; private set; }

		public override string ToString() => $"{this.Name} : {this.Type.Name}";
	}

	public sealed class ArrayType : BasicType
	{
		public BasicType UnderlyingType { get; set; }
	}
}