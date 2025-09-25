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

		public bool IsIndexSelected(int index) => _assetList.IsSelected(index);
		
		public bool DrawTypeIcon { get; set; }

		public string DefaultTypeIconGUID
		{
			get
			{
				return _defaultIconGuidGuid;
			}
			set
			{
				_defaultIconGuidGuid = value;

				if (_defaultIconGuidGuid == null)
				{
					_defaultTypeIcon = null;
					return;
				}
				
				var iconGuid = value;
				_defaultTypeIcon = new Lazy<Texture>(() =>
				{
					var path = AssetDatabase.GUIDToAssetPath(iconGuid);
					return AssetDatabase.LoadAssetAtPath<Texture>(path);
				});
			}
		}

		public NestedAssetList(SP arrayProp)
		{
			_arrayProp = arrayProp;
			_addContext = UtilityEditorUtils.CreateTypeMenu(typeof(T), OnAddOption);
			_assetList = new RL(_arrayProp.serializedObject, _arrayProp);
			_assetList.onAddDropdownCallback = (r, l) => _addContext.DropDown(r);
			_assetList.onRemoveCallback = OnDeleteAsset;

			_assetList.drawHeaderCallback = rect =>
			{
				EditorGUI.LabelField(rect, new  GUIContent(_arrayProp.displayName), EditorStyles.whiteLargeLabel);
			};

			_assetList.onSelectCallback = list =>
			{
				// notify owner?
			};

			_assetList.drawElementCallback = (rect, index, isActive, isFocused) =>
			{
				DrawListItem(rect, index);
			};
		}

		// 
		public void OnListGUI()
		{
			if (_assetList == null)
			{
				return;
			}

			_arrayProp.serializedObject.UpdateIfRequiredOrScript();
			_assetList.DoLayoutList();
			_arrayProp.serializedObject.ApplyModifiedProperties();

			EnsureInspector();

			if (_childInspector && _childInspector.target)
			{
				EditorGUILayout.Space(2);
				// DrawScriptInfo();
				if (_childName != null)
				{
					EditorGUILayout.BeginVertical(GUI.skin.box);
					EditorGUILayout.PropertyField(_childName);
					EditorGUILayout.EndVertical();
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

		private RL _assetList = null;
		private SP _arrayProp = null;
		private GenericMenu _addContext = null;
		private Editor _childInspector = null;
		private SP _childName = null;
		private string _defaultIconGuidGuid = null;
		private Lazy<Texture> _defaultTypeIcon = null;


		private void DrawScriptInfo()
		{
			if (!_childInspector)
			{
				return;
			}

			var type = _childInspector.target.GetType();

			var typeLabel = $"{type.Assembly.GetName().Name}.{type.Name}";

			var open = false;

			var tempColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.cyan * 0.5f;
			
			EditorGUILayout.BeginVertical(GUI.skin.box);

			var btnLabel = new GUIContent("Edit");
			var btnStyle = EditorStyles.miniButton;
			var btnWidth = btnStyle.CalcSize(btnLabel).x;
			
			var dRect = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight));
			dRect.SliceLeft(2f);

			var iconRect = dRect.SliceLeft(dRect.height);
			dRect.SliceLeft(2f);
			
			var btnRect = dRect.SliceRight(btnWidth);
			dRect.SliceRight(2f);
			
			DrawIconBasic(iconRect, _childInspector.target as ScriptableObject);
			
			EditorGUI.LabelField(dRect, typeLabel, EditorStyles.miniLabel);

			open = GUI.Button(btnRect, btnLabel, btnStyle);

			EditorGUILayout.EndVertical();

			GUI.backgroundColor = tempColor;

			if (open)
			{
				UtilityEditorUtils.OpenScriptEditor(_childInspector.target);
			}
			
		}

		private static Lazy<Texture2D> _contextButtonIcon = new Lazy<Texture2D>(() =>
		{
			return EditorGUIUtility.FindTexture("_Menu");
		});

		private void DrawContextButton(Rect rect, T asset)
		{
			// EditorGUI.DrawRect(rect, Color.black);
			if (GUI.Button(rect, new GUIContent(_contextButtonIcon.Value), EditorStyles.iconButton))
			{
				ShowContextMenu(asset);
			}
			
		}

		private void ShowContextMenu(T asset)
		{
				var scriptFile = UtilityEditorUtils.GetObjectMonoscript(asset);

				var fileName = scriptFile.name;

				var m = new GenericMenu();
			
				m.AddItem(new GUIContent($"Edit Script"), false, () =>
				{
					AssetDatabase.OpenAsset(scriptFile);
				});

				m.ShowAsContext();
			// };

		}

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
			var i = _assetList.index;
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
			rect.SliceLeft(2f);
			rect.SliceRight(2f);
			
			SP prop = _arrayProp.GetArrayElementAtIndex(index);
			
			var asset = prop.objectReferenceValue as T;

			// if (Event.current != null && Event.current.isMouse && Event.current.button == 1 && rect.Contains(Event.current.mousePosition))
			// {
			// 	ShowContextMenu(asset);
			// }


			
			
			if (DrawTypeIcon)
			{
				var iconRect = rect.SliceLeft(rect.height);
				rect.SliceLeft(1);
				DrawIcon(iconRect, asset);
			}

			var ctxRect = rect.SliceRight(rect.height * 0.6f);
			rect.SliceRight(5f);
			DrawContextButton(ctxRect, asset);

			if (asset && onDrawListItem != null)
			{
				var labelRect = rect;
				labelRect.height = EditorGUIUtility.singleLineHeight;
				labelRect.center = rect.center;
				onDrawListItem.Invoke(labelRect, prop, asset);
				return;
			}
			else
			{
				EditorGUI.LabelField(rect, asset?.name ?? "null");
				
			}
		}

		private void DrawIcon(Rect rect, ScriptableObject asset)
		{
			rect.Resize(-2f);
			
			var ms = MonoScript.FromScriptableObject(asset);
			var path = AssetDatabase.GetAssetPath(ms);
			Texture ico = AssetDatabase.GetCachedIcon(path);

			if (_defaultTypeIcon != null && UtilityEditorUtils.IsDefaultScriptIcon(ico))
			{
				ico = _defaultTypeIcon.Value;
			}

			if (!ico)
			{
				return;
			}
			GUI.DrawTexture(rect, ico, ScaleMode.StretchToFill);
		}
		
		private static void DrawIconBasic(Rect rect, ScriptableObject asset)
		{
			rect.Resize(-2f);
			
			var ms = MonoScript.FromScriptableObject(asset);
			var path = AssetDatabase.GetAssetPath(ms);
			Texture ico = AssetDatabase.GetCachedIcon(path);

			if (!ico)
			{
				return;
			}
			GUI.DrawTexture(rect, ico, ScaleMode.StretchToFill);
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