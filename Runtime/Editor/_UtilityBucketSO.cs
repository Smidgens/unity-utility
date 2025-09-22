// smidgens @ github

// ReSharper disable All

using System.Linq;

#if UNITY_EDITOR

namespace Smidgenomics.Unity.UtilityAI.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using UObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using RL = UnityEditorInternal.ReorderableList;

	[CustomEditor(typeof(UtilityBucketSO))]
	internal class _UtilityBucketSO : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			EditorGUILayout.Space(5f);
			// base.OnInspectorGUI();
			_actionAssetList.OnListGUI();
			serializedObject.ApplyModifiedProperties();
		}

		private NestedAssetList<UtilityActionSO> _actionAssetList = null;

		private void OnEnable()
		{
			var listProp = serializedObject.FindProperty(nameof(UtilityBucketSO._actions));
			_actionAssetList = new NestedAssetList<UtilityActionSO>(listProp);

			_actionAssetList.onDrawListItem = (rect, prop, so) =>
			{
				var iconRect = rect.SliceLeft(rect.height);
				rect.SliceLeft(2f);

				DrawTypeIcon(iconRect, so);
				
				var crect = rect.SliceRight(rect.height * 2f);
				rect.SliceRight(2f);
				var wrect = rect.SliceRight(rect.height * 2f);
				rect.SliceRight(2f);
				var count = CountEnabledConsiderations(so);
				EditorGUI.LabelField(wrect, "w=" + StringifyFloat(so._weight));
				EditorGUI.LabelField(crect, "c=" + count.ToString());
				EditorGUI.LabelField(rect, so.name);
			};
		}

		private void OnDisable()
		{
			// cleanup
			if (_actionAssetList != null)
			{
				_actionAssetList.DisposeGUI();
			}
		}

		private static void DrawTypeIcon(Rect rect, ScriptableObject asset)
		{
			rect.Resize(-2f);
			
			var ms = MonoScript.FromScriptableObject(asset);
			var path = AssetDatabase.GetAssetPath(ms);
			// var path = AssetDatabase.GetAssetPath(asset);
			Texture ico = AssetDatabase.GetCachedIcon(path);

			// default so icon: "d_ScriptableObject Icon"

			if (!ico)
			{
				return;
			}
			GUI.DrawTexture(rect, ico, ScaleMode.StretchToFill);
		}

		private static string StringifyFloat(float v)
		{
			return v.ToString("0.0");
		}
		
		private static int CountEnabledConsiderations(UtilityActionSO action)
		{
			int c = 0;
			foreach(var consideration in action._considerations)
			{
				if (consideration.Enabled)
				{
					c++;
				}
			}
			return c;
		}

	}
}

#endif