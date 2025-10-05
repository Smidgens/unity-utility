// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;

	[CreateAssetMenu(menuName = "Utility/Consideration Set")]
	public sealed class UtilityConsiderationSetSO : ScriptableObject
	{
		public float GetScore(in UtilityContext context)
		{
			var score = UtilityMath.ScoreConsiderations(context, _considerations, out var count);
			return count == 0 ? 0 : score;
		}
	
		[HideInInspector]
		[SerializeField]
		internal UtilityConsiderationSO[] _considerations = { };
	}
}