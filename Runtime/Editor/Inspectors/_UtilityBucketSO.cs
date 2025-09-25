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
			_actionAssetList.OnListGUI();
			serializedObject.ApplyModifiedProperties();
		}

		protected override bool ShouldHideOpenButton()
		{
			return true;
		}

		private NestedAssetList<UtilityActionSO> _actionAssetList = null;

		private void OnEnable()
		{
			var listProp = serializedObject.FindProperty(nameof(UtilityBucketSO._actions));
			_actionAssetList = new NestedAssetList<UtilityActionSO>(listProp);

			_actionAssetList.DefaultTypeIconGUID = "b403041b6ec9a3744b4e92bc8014f7f6";
			_actionAssetList.DrawTypeIcon = true;

			_actionAssetList.onDrawListItem = (rect, prop, so) =>
			{

				var checkRect = rect.SliceLeft(rect.height);
				
				var newEnabled = GUI.Toggle(checkRect, so._enabled, GUIContent.none);
				if (newEnabled != so._enabled)
				{
					Undo.RecordObject(so, "Toggle enabled");
					so._enabled = newEnabled;
				}
				
				var wrect = rect.SliceRight(50);
				var newWeight = Mathf.Max(EditorGUI.FloatField(wrect, so._weight), 0f);
				if (newWeight != so._weight)
				{
					Undo.RecordObject(so, "Change weight");
					so._weight = newWeight;
				}
				
				var crect = rect.SliceRight(rect.height * 2f);
				rect.SliceRight(2f);
				// var wrect = rect.SliceRight(rect.height * 2.5f);
				// rect.SliceRight(2f);
				var count = CountEnabledConsiderations(so);
				// EditorGUI.LabelField(wrect, "w=" + StringifyFloat(so._weight));
				EditorGUI.LabelField(crect, $"c({count})");
				
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

		private static string StringifyFloat(float v)
		{
			return v.ToString("0.0");
		}

		private static int CountEnabledConsiderations(UtilityActionSO action)
		{
			int c = 0;
			foreach(var consideration in action._considerations)
			{
				if (consideration != null && consideration.Enabled)
				{
					c++;
				}
			}
			return c;
		}
		

	}
}

#endif