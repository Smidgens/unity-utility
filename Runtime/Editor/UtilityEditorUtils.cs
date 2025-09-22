// smidgens @ github

// ReSharper disable All

using System.Reflection;

#if UNITY_EDITOR

namespace Smidgenomics.Unity.UtilityAI.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Linq;

	/**
	 *
	 */
	internal static class UtilityEditorUtils
	{

		public static Type[] GetDerivedTypes(Type baseType)
		{
			return baseType.Assembly.GetTypes()
				.Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract)
				.ToArray();
		}
        
		public static GenericMenu CreateTypeMenu(Type baseType, GenericMenu.MenuFunction2 fn)
		{
			var menu = new GenericMenu();
			var types = UtilityEditorUtils.GetDerivedTypes(baseType);
			foreach (var type in types)
			{
				var assemblyName = type.Assembly.GetName().Name;
				// var dname = new GUIContent($"{assemblyName}/{type.Name}");
				var dname = GetTypeLabel(type);
				menu.AddItem(dname, false, fn,  type);
			}
			return menu;
		}

		public static void ReimportAsset(UnityEngine.Object asset)
		{
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
		}

		private static GUIContent GetTypeLabel(Type type)
		{
			string category = null;
			string dname = null;

			var md = type.GetCustomAttribute<UtilityAssetMD>();

			if (md != null)
			{
				category = md.category;
				dname = md.displayName;
			}

			if (dname == null)
			{
				dname = type.Name;
			}
			
			var path = category != null ? category + "/" + dname : dname;
			
			return new GUIContent(path);
		}

	}
}

#endif