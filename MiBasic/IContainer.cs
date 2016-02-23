using System.Collections.Generic;

namespace MiBasic
{
	public interface IContainer<T> : IEnumerable<T> where T : class, INamedObject
	{
		T this[string name] { get; }
	}
}