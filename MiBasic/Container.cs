using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiBasic
{
	public sealed class Container<T> : IContainer<T> where T : class, INamedObject
	{
		private readonly Dictionary<string, T> types = new Dictionary<string, T>();

		public Container()
		{

		}

		public void Register(T type)
		{
			this.Register(type, false);
		}

		public void Register(T type, bool allowOverride)
		{
			if (allowOverride)
				this.types[type.Name] = type;
			else
				this.types.Add(type.Name, type);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.types.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public T this[string name] => this.types.FirstOrDefault(t => t.Key.IsSemanticEqualTo(name)).Value; 
	}

	public interface INamedObject
	{
		string Name { get; }
	}
}