// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using UnityEngine.Serialization;

	public abstract class UtilityConsiderationSO : UtilitySO, IUtilityConsideration
	{
		// public bool Enabled => _enabled;

		public abstract float GetScore(in UtilityContext Context);

		protected float EvalScore(float score)
		{
			score = Mathf.Clamp01(_curve.Evaluate(score));
			return _invert ? 1 - score : score;
		}

		// [HideInInspector]
		// [SerializeField] internal bool _enabled = true;

		[HideInInspector]
		[SerializeField] internal bool _invert = false;
		
		[HideInInspector]
		[FormerlySerializedAs("_responseCurve")]
		[SerializeField] internal AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
	}
}