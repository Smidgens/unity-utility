// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using System;
	using UnityEngine;

	internal static class UtilityIMGUI
	{
		
		public static void DrawRect(in Rect rect, Color color)
		{
			var tColor = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(rect, WhiteTex.Value);
			GUI.color = tColor;
		}

		public static void DrawLabel(in Rect rect, string text, Color color, GUIStyle style = null)
		{
			var tColor = GUI.color;
			GUI.color = color; 
			GUI.Label(rect, text, style ?? GUI.skin.label);
			GUI.color = tColor; 
		}

		private static readonly Lazy<Texture2D> WhiteTex = new(() =>
		{
			var t = new Texture2D(1,1);
			t.SetPixel(0,0, Color.white);
			t.Apply();
			return t;
		});
	}
}