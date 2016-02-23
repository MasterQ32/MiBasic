using System;

namespace MiBasic.Parser
{
	public sealed class BasicTypeName : IEquatable<BasicTypeName>
	{
		public BasicTypeName(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			this.Name = name;
		}

		public string Name { get; private set; }

		public override int GetHashCode() => this.Name.GetHashCode();

		public override bool Equals(object obj) => this.Equals(obj as BasicTypeName);

		public bool Equals(BasicTypeName other)
		{
			if (other == null) return false;
			return this.Name.IsSemanticEqualTo(other.Name);
		}

		public static bool operator == (BasicTypeName lhs, BasicTypeName rhs)
		{
			if ((object)lhs == null)
				return ((object)rhs == null);
			return lhs.Equals(rhs);
		}

		public static bool operator !=(BasicTypeName lhs, BasicTypeName rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString() => this.Name;
	}
}