// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;

	//[CreateAssetMenu]
	[UtilityAssetMD(displayName = "Random")]
	internal sealed class UAIC_Random : UtilityConsiderationSO
	{
		public override float GetScore(in UtilityContext Context)
		{
			var mn = Mathf.Min(_min, _max);
			var mx = Mathf.Max(_min, _max);
			return EvalScore(UnityEngine.Random.Range(mn, mx));
		}

		[Range(0, 1)]
		[SerializeField] private float _min = 0, _max = 1f;
	}
}