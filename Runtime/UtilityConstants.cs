// smidgens @ github

// ReSharper disable All

#pragma warning disable 0414
#pragma warning disable 0067

namespace Smidgenomics.Unity.UtilityAI
{
	using Color = UnityEngine.Color;

	internal static class UtilityConstants
	{
		public const float MIN_COOLDOWN = 0.01f;
		public static readonly Color COLOR_ACTION_ACTIVE = Color.green;
		public static readonly Color COLOR_ACTION_MUTED = new Color(0.7f, 0.7f, 0.7f);
		public static readonly Color COLOR_ACTION_DEACTIVATING = Color.yellow;
		public static readonly Color COLOR_ACTION_CANCELLED = Color.blue;
		public static readonly Color COLOR_ACTION_FINISHED = Color.cyan;
		public static readonly Color COLOR_ACTION_UNCANCELLABLE = Color.magenta;
		public static readonly Color COLOR_SELECTABLE = Color.white;
	}
}