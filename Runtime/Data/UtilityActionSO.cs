// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;

	/**
	 * TODO:
	 * - Considerations
	 */
	public abstract class UtilityActionSO : ScriptableObject, IUtilityAction
	{
		public virtual System.Collections.IEnumerator OnActivate(UtilityContext context)
		{
			yield return null;
		}

		public virtual void OnTick(in UtilityContext context)
		{
			// update
		}

		public virtual void OnEnd(in UtilityContext context)
		{
			// 
		}

		// 
		public float GetScore(in UtilityContext context)
		{
			if (Mathf.Approximately(_weight, 0f))
			{
				return 0f;
			}
			var conScore = UtilityMath.ScoreConsiderations(context, _considerations, out int Count);
			return _weight * conScore;
		}

		event Action IUtilityAction.OnActionFinished
		{
			add => _onActionFinished += value;
			remove => _onActionFinished-= value;
		}

		IUtilityAction IUtilityAction.InstantiateAction()
		{
			return ScriptableObject.Instantiate(this);
		}

		private Action _onActionFinished;

		[Range(0,1)]
		[SerializeField] internal float _weight = 1f;

		[HideInInspector]
		[SerializeField] internal UtilityConsiderationSO[] _considerations = { };
	}
}