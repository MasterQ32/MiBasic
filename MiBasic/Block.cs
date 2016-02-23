using System.Collections;
using System.Collections.Generic;

namespace MiBasic
{
	public sealed class Block : IList<Instruction>
	{
		private readonly List<Instruction> instructions = new List<Instruction>();

		public Block()
		{
		}

		public void Sanitize(CodeEnvironment env)
		{
			foreach(var instr in this)
			{
				instr.Sanitize(env);
			}
		}

		public Instruction this[int index]
		{
			get
			{
				return instructions[index];
			}

			set
			{
				instructions[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return instructions.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((IList<Instruction>)instructions).IsReadOnly;
			}
		}

		public void Add(Instruction item)
		{
			instructions.Add(item);
		}

		public void Clear()
		{
			instructions.Clear();
		}

		public bool Contains(Instruction item)
		{
			return instructions.Contains(item);
		}

		public void CopyTo(Instruction[] array, int arrayIndex)
		{
			instructions.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Instruction> GetEnumerator()
		{
			return ((IList<Instruction>)instructions).GetEnumerator();
		}

		public int IndexOf(Instruction item)
		{
			return instructions.IndexOf(item);
		}

		public void Insert(int index, Instruction item)
		{
			instructions.Insert(index, item);
		}

		public bool Remove(Instruction item)
		{
			return instructions.Remove(item);
		}

		public void RemoveAt(int index)
		{
			instructions.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<Instruction>)instructions).GetEnumerator();
		}
	}
}