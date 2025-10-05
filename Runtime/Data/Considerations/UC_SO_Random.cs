// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	
	[DisplayName("Random Range")]
	internal sealed class UC_SO_Random : UtilityConsiderationSO
	{
		public override float GetScore(in UtilityContext Context)
		{
			var mn = Mathf.Min(_interval.min, _interval.max);
			var mx = Mathf.Max(_interval.min, _interval.max);
			return EvalScore(Random.Range(mn, mx));
		}

		[FloatInterval(0, 1)]
		[SerializeField] private FloatInterval _interval = new() { min = 0, max = 1 };
	}
}