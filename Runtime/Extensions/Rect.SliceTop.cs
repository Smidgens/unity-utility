// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;

	internal static partial class Rect_
	{
		/// <summary>
		/// Slices a rect of height h from top and returns it.
		/// Note: changes original rect
		/// </summary>
		/// <param name="r">Rect</param>
		/// <param name="h">Slice height</param>
		/// <returns>Top slice</returns>
		public static Rect SliceTop(this ref Rect r, in float h)
		{
			var r2 = r;
			r2.height = h;
			r.height -= h;
			r.position += new Vector2(0f, h);
			return r2;
		}
	}
}

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;

	internal static partial class Rect_
	{
		/// <summary>
		/// Slices a rect of height h from bottom and returns it.
		/// Note: changes original rect
		/// </summary>
		/// <param name="r">Rect</param>
		/// <param name="h">Slice height</param>
		/// <returns>Bottom slice</returns>
		public static Rect SliceBottom(this ref Rect r, in float h)
		{
			var r2 = r;
			r2.height = h;
			r.height -= h;
			r2.y += r.height;
			return r2;
		}
	}
}