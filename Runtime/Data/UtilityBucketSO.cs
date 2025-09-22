// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;

	/**
	 * TODO:
	 * - Actions
	 */
	[CreateAssetMenu(menuName = "Utility/Utility Bucket")]
	internal sealed class UtilityBucketSO : ScriptableObject
	{
		[HideInInspector]
		[SerializeField] internal UtilityActionSO[] _actions = { };
	}
}