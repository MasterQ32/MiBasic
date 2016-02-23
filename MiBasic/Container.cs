using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiBasic
{
	public sealed class Container<T> : IContainer<T> where T : class, INamedObject
	{
		private readonly Dictionary<string, T> contents = new Dictionary<string, T>();

		private IContainer<T> @base = null;

		public Container()
		{

		}

		/// <summary>
		/// Creates a container with a derivative container that is used for sourcing if no entry is found.
		/// </summary>
		/// <param name="base"></param>
		public Container(IContainer<T> @base)
		{
			this.@base = @base;
		}

		/// <summary>
		/// Adds a new entry to the container. Throws an exception if a entry with the same name already exists.
		/// </summary>
		/// <param name="entry"></param>
		public void Register(T entry)
		{
			this.Register(entry, false);
		}

		/// <summary>
		/// Adds a new entry to the container.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="allowOverride">If false the method throws an exception if a entry with the same name already exists.</param>
		public void Register(T entry, bool allowOverride)
		{
			if (allowOverride)
				this.contents[entry.Name] = entry;
			else
				this.contents.Add(entry.Name, entry);
		}

		public IEnumerator<T> GetEnumerator()
		{
			if(this.@base == null)
				return this.contents.Values.GetEnumerator();

			var entries = this.contents.Values.ToArray();

			return entries.Concat(this.@base.Except(entries, new NamedObjectComparer<T>())).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Gets an entry from the container. If the container is derived by another container, the base container is also searched.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public T this[string name] => 
			this.contents.FirstOrDefault(t => t.Key.IsSemanticEqualTo(name)).Value ??
			this.@base?[name];
	}

	public interface INamedObject
	{
		string Name { get; }
	}

	public sealed class NamedObjectComparer<T> : IComparer<T>, IEqualityComparer<T>
		where T : INamedObject
	{
		public int Compare(T x, T y)
		{
			return x.Name.CompareTo(y?.Name);
		}

		public bool Equals(T x, T y)
		{
			return x?.Name == y?.Name;
		}

		public int GetHashCode(T obj)
		{
			return obj.Name.GetHashCode();
		}
	}
}