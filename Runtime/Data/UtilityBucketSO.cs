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
		public string BucketName => _name;

		[SerializeField] internal string _name = "Bucket";
		
		[Min(0.1f)]
		[SerializeField] internal float _actionScoringInterval = 1f;

		[HideInInspector]
		[SerializeField] internal UtilityActionSO[] _actions = { };
	}
}