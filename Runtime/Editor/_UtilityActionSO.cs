// smidgens @ github

// ReSharper disable All

#if UNITY_EDITOR

namespace Smidgenomics.Unity.UtilityAI.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using RL = UnityEditorInternal.ReorderableList;

	[CustomEditor(typeof(UtilityActionSO), true)]
	internal class _UtilityActionSO : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();

			foreach (var prop in _props)
			{
				if (prop == null)
				{
					EditorGUILayout.Space();
					continue;
				}
				EditorGUILayout.PropertyField(prop);
			}
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Space();
			_considerationView.OnListGUI();
			serializedObject.ApplyModifiedProperties();
		}

		private NestedAssetList<UtilityConsiderationSO> _considerationView = null;
		private List<SP> _props = new();

		private void OnEnable()
		{
			_props = new List<SP>();
			foreach (var f in target.GetType().FindInspectorFields())
			{
				var prop = serializedObject.FindProperty(f.Name);
				_props.Add(prop);
			}
			var listProp = serializedObject.FindProperty(nameof(UtilityActionSO._considerations));
			_considerationView = new (listProp);

			_considerationView.onDrawListItem = (rect, prop, so) =>
			{
				var tickRect = rect.SliceLeft(rect.height);
				tickRect.Resize(-5f);
				rect.SliceLeft(2f);
				
				var color = so.Enabled ? Color.green : Color.gray;
	
				EditorGUI.DrawRect(tickRect, color * 0.8f);

				var curveRect = rect.SliceRight(rect.height * 1.5f);
				curveRect.Resize(-2f);
				rect.SliceRight(2f);

				var tenabled = GUI.enabled;
				GUI.enabled = false;
				EditorGUI.CurveField(curveRect, so._curve);
				GUI.enabled = tenabled;
				EditorGUI.LabelField(rect, so.name);
			};
		}

		private void OnDisable()
		{
			if (_considerationView != null)
			{
				_considerationView.DisposeGUI();
			}
		}

	}
}

#endif