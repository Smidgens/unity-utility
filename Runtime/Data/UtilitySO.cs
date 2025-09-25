// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;

	[ExcludeFromPreset]
	public abstract class UtilitySO : ScriptableObject
	{
		public string Name => name;

		public bool Enabled => _enabled;
		
		[HideInInspector]
		[SerializeField] internal bool _enabled = true;
		[HideInInspector]
		[SerializeField] internal string _internalID = System.Guid.NewGuid().ToString().Replace("-", "");
	}
}