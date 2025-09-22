// smidgens @ github

// ReSharper disable All

#if UNITY_EDITOR

namespace Smidgenomics.Unity.UtilityAI.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using UAsset = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;
	using RL = UnityEditorInternal.ReorderableList;

	internal sealed class NestedAssetList<T> where T : ScriptableObject
	{
		public Action<Rect, SP, T> onDrawListItem = null;
		
		
		public NestedAssetList(SP arrayProp)
		{
			_arrayProp = arrayProp;
			_addContext = UtilityEditorUtils.CreateTypeMenu(typeof(T), OnAddOption);
			_actionList = new RL(_arrayProp.serializedObject, _arrayProp);
			_actionList.onAddDropdownCallback = (r, l) => _addContext.DropDown(r);
			_actionList.onRemoveCallback = OnDeleteAsset;

			_actionList.headerHeight = EditorGUIUtility.singleLineHeight * 1.5f;

			_actionList.drawHeaderCallback = rect =>
			{
				EditorGUI.LabelField(rect, new  GUIContent(_arrayProp.displayName), EditorStyles.whiteLargeLabel);
			};

			_actionList.onSelectCallback = list =>
			{

			};

			_actionList.drawElementCallback = (rect, index, isActive, isFocused) =>
			{
				DrawListItem(rect, index);
			};

		}

		// 
		public void OnListGUI()
		{
			if (_actionList == null)
			{
				return;
			}

			_arrayProp.serializedObject.UpdateIfRequiredOrScript();
			_actionList.DoLayoutList();
			_arrayProp.serializedObject.ApplyModifiedProperties();

			EnsureInspector();
			
			if (_childInspector && _childInspector.target)
			{
				EditorGUILayout.Space(5);
				
				if (_childName != null)
				{
					EditorGUILayout.PropertyField(_childName);
					EditorGUILayout.Space();

					_childName.serializedObject.ApplyModifiedProperties();
				}
				_childInspector.OnInspectorGUI();
			}
		}

		public void DisposeGUI()
		{
			if (_childInspector)
			{
				Editor.DestroyImmediate(_childInspector);
				_childInspector = null;
				_childName = null;
			}
		}

		private RL _actionList = null;
		private SP _arrayProp = null;
		private GenericMenu _addContext = null;
		private Editor _childInspector = null;
		private SP _childName = null;

		private void OnAddOption(object option)
		{
			EditorApplication.delayCall += () => AddAsset(option as Type, _arrayProp);
		}

		private void DelayCall(Action action)
		{
			EditorApplication.delayCall += () => action.Invoke();
		}

		private void EnsureInspector()
		{
			var i = _actionList.index;
			var currentItem = i >= 0 && i < _arrayProp.arraySize
			? _arrayProp.GetArrayElementAtIndex(i).objectReferenceValue
			: null;
			
			if (_childInspector && (_childInspector.target != currentItem || !_childInspector.target))
			{
				UAsset.DestroyImmediate(_childInspector);
				_childInspector = null;
				_childName = null;
			}

			if (!_childInspector && currentItem)
			{
				_childInspector = Editor.CreateEditor(currentItem);
				_childName = _childInspector.serializedObject.FindProperty("m_Name");
			}
		}

		private void DrawListItem(Rect rect, int index)
		{
			var labelRect = rect;
			labelRect.height = EditorGUIUtility.singleLineHeight;
			SP prop = _arrayProp.GetArrayElementAtIndex(index);
			
			var asset = prop.objectReferenceValue;

			if (onDrawListItem != null)
			{
				onDrawListItem.Invoke(rect, prop, asset as T);
				return;
			}
			
			if (!asset)
			{
				EditorGUI.LabelField(labelRect, new GUIContent("(none)"));
				return;
			}
			EditorGUI.LabelField(labelRect, new GUIContent(asset.name));
		}

		private void OnDeleteAsset(RL list)
		{
			var i = list.index;
			var sp = list.serializedProperty;
			UAsset asset = _arrayProp.GetArrayElementAtIndex(i).objectReferenceValue;
			var path = AssetDatabase.GetAssetPath(asset);
			var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
			sp.DeleteArrayElementAtIndex(i);
			sp.serializedObject.ApplyModifiedProperties();
			Undo.DestroyObjectImmediate(asset);
		}

		// adds a new SO asset of given type to main asset and inserts it to array
		private void AddAsset(Type assetType, SerializedProperty arrayProp)
		{
			var mainAsset = arrayProp.serializedObject.targetObject as UAsset;
			var newAsset = ScriptableObject.CreateInstance(assetType);

			newAsset.hideFlags = HideFlags.HideInHierarchy;
			
			newAsset.name = assetType.Name;
			Undo.RegisterCreatedObjectUndo(newAsset, "Create child asset");
			AssetDatabase.AddObjectToAsset(newAsset, mainAsset);
			var newIndex = arrayProp.arraySize;
			arrayProp.InsertArrayElementAtIndex(newIndex);
			arrayProp.GetArrayElementAtIndex(newIndex).objectReferenceValue = newAsset;
			arrayProp.serializedObject.ApplyModifiedProperties();	
			// UtilityEditorUtils.ReimportAsset(mainAsset);
		}


	}
	
	
}

#endif