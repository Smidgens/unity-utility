// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System.Collections.Generic;

	/**
	 * TODO:
	 * - Actions
	 */
	[CreateAssetMenu(menuName = "Utility/Utility Bucket")]
	[ExcludeFromPreset]
	public sealed class UtilityBucketSO : ScriptableObject
	{
		public IEnumerable<IUtilityAction> GetActions() => _actions;


		[HideInInspector]
		[SerializeField] internal UtilityActionSO[] _actions = { };
	}
}