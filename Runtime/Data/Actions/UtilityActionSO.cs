// smidgens @ github

// ReSharper disable All

#pragma warning disable 0414
#pragma warning disable 0067

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using IEnumerator = System.Collections.IEnumerator;

	/**
	 * TODO:
	 * - Considerations
	 */
	public abstract class UtilityActionSO : UtilitySO, IUtilityAction
	{
		EUtilityActionStatus IUtilityAction.Status
		{
			get => _status;
			set => _status = value;
		}

		public EUtilityActionStatus GetActionStatus()
		{
			return _status;
		}

		public virtual float GetActionCooldown()
		{
			return 1;
		}

		public virtual IEnumerator ActivateAction()
		{
			return null;
		}

		public virtual IEnumerator DeactivateAction()
		{
			return null;
		}

		public virtual bool CanCancelAction()
		{
			return true;
		}

		// 
		public float GetTotalScore()
		{
			if (Mathf.Approximately(_weight, 0f))
			{
				return 0f;
			}
			var conScore = UtilityMath.ScoreConsiderations(_currentContext, _considerations, out int Count);
			return _weight * conScore;
		}

		protected void FinishAction()
		{
			_callbacks.onActionFinished?.Invoke();
		}

		protected UtilityContext GetContext()
		{
			return _currentContext;
		}

		IUtilityAction IUtilityAction.InstantiateAction(UtilityContext context, UtilityActionCallbacks callbacks)
		{
			var instance = ScriptableObject.Instantiate(this);
			instance.name = name;
			instance._currentContext = context;
			instance._callbacks = callbacks;
			return instance;
		}

		[Min(0f)]
		[HideInInspector]
		[SerializeField] internal float _weight = 1f;

		[HideInInspector]
		[SerializeField] internal UtilityConsiderationSO[] _considerations = { };

		private UtilityContext _currentContext = default;
		private UtilityActionCallbacks _callbacks = default;
		private EUtilityActionStatus _status = EUtilityActionStatus.Inactive;
	}
}