using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

public static class HashSetExtensions
{
	public static HashSet<T> Clone<T>(this HashSet<T> original)
	{
		var clone = (HashSet<T>)FormatterServices.GetUninitializedObject(typeof(HashSet<T>));
		Copy(Fields<T>.comparer, original, clone);

		if (original.Count == 0)
		{
			Fields<T>.freeList.SetValue(clone, -1);
		}
		else
		{
			Fields<T>.count.SetValue(clone, original.Count);
			Clone(Fields<T>.buckets, original, clone);
			Clone(Fields<T>.slots, original, clone);
			Copy(Fields<T>.freeList, original, clone);
			Copy(Fields<T>.lastIndex, original, clone);
			Copy(Fields<T>.version, original, clone);
		}

		return clone;
	}

	static void Copy<T>(FieldInfo field, HashSet<T> source, HashSet<T> target)
	{
		field.SetValue(target, field.GetValue(source));
	}

	static void Clone<T>(FieldInfo field, HashSet<T> source, HashSet<T> target)
	{
		field.SetValue(target, ((Array)field.GetValue(source)).Clone());
	}

	static class Fields<T>
	{
		public static readonly FieldInfo freeList = GetFieldInfo("m_freeList");
		public static readonly FieldInfo buckets = GetFieldInfo("m_buckets");
		public static readonly FieldInfo slots = GetFieldInfo("m_slots");
		public static readonly FieldInfo count = GetFieldInfo("m_count");
		public static readonly FieldInfo lastIndex = GetFieldInfo("m_lastIndex");
		public static readonly FieldInfo version = GetFieldInfo("m_version");
		public static readonly FieldInfo comparer = GetFieldInfo("m_comparer");

		static FieldInfo GetFieldInfo(string name)
		{
			return typeof(HashSet<T>).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
		}
	}
}