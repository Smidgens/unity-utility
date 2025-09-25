// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;

	[DisplayName("Sin Wave")]
	internal sealed class UC_SO_SinWave : UtilityConsiderationSO
	{
		public override float GetScore(in UtilityContext Context)
		{
			return EvalScore((Mathf.Sin(Time.time * _speed) + 1f) / 2f);
		}

		[SerializeField] private float _speed = 100f;
	}
}