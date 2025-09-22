// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.UtilityAI.Editor
{
	using System;
	using UnityEngine;
	using System.Reflection;
	using System.Collections.Generic;

	public static partial class _Type_
	{
		/// <summary>
		/// [Editor] Find all fields that Unity would default render in the inspector
		/// </summary>
		public static IEnumerable<FieldInfo> FindInspectorFields<T>(this Type owner) where T : Component
		{
			// NOTE: doesn't work properly for unity components, flags might need to be different

			var baseType = typeof(T);

			List<FieldInfo> fields = new List<FieldInfo>();
			LinkedList<Type> hierarchy = new LinkedList<Type>(); // linked for efficient prepend

			// traverse parent hierarchy, stop at MonoBehaviour
			Type currentType = owner;
			while (currentType != baseType && currentType != null)
			{
				hierarchy.AddFirst(currentType);
				currentType = currentType.BaseType;
			}

			// append fields in
			// same order as Unity would normally list them
			foreach (Type htype in hierarchy)
			{
				foreach (FieldInfo field in htype.GetFields(FIELD_FLAGS))
				{
					if (!IsInspectorField(field)) { continue; }
					fields.Add(field);
				}
			}
			return fields;
		}


		/// <summary>
		/// Find all fields that would be rendered by unity in the inspector
		/// </summary>
		public static IEnumerable<FieldInfo> FindInspectorFields(this Type owner)
		{
			return FindInspectorFields<Component>(owner);
		}

		// can field be drawn by inspector
		private static bool IsInspectorField(FieldInfo f)
		{
			// explicitly public but non-serialized
			if (f.IsPublic && f.GetCustomAttribute<NonSerializedAttribute>() != null) { return false; }

			// explicitly hidden
			if (f.GetCustomAttribute<HideInInspector>() != null) { return false; }

			// private, non serialized
			if (!f.IsPublic && f.GetCustomAttribute<SerializeField>() == null) { return false; }

			// at this point, either the field is public, or private and using SerializeField
			return true;
		}

		// instance fields declared by type
		private static BindingFlags FIELD_FLAGS =
		BindingFlags.NonPublic
		| BindingFlags.Public
		| BindingFlags.DeclaredOnly
		| BindingFlags.Instance;

	}

}

#endif