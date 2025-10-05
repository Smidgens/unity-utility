// smidgens @ github

// ReSharper disable All

#pragma warning disable 0414
#pragma warning disable 0067

namespace Smidgenomics.Unity.UtilityAI
{
	public delegate void ActionRefRO<T>(in T item);
	public delegate R FuncRefRO<T, R>(in T item);
}


namespace Smidgenomics.Unity.UtilityAI
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using System.Linq;
	using IEnumerator = System.Collections.IEnumerator;

	internal class UtilityScoringHandler
	{
		// todo: pass scoring context in here
	}

	/**
	 * TODO:
	 * - Tick brain
	 * - Current action
	 */
	public sealed class UtilityBrain
	{

		public IUtilityAction CurrentTemplate => GetCurrentActionTemplate();

		public event Action<IUtilityAction> onActionChanged = null;

		public UtilityContext GetContext() => _context;
		

		public static UtilityBrain CreateBrain(in UtilityBrainInitConfig config)
		{
			var brain = new UtilityBrain();
			brain._context = config.context;
			brain._buckets = config.buckets ?? Array.Empty<BucketExecutionConfig>();
			return brain;
		}

		// 
		public int GetCurrentActionID() => _currentActionID;

		public int GetCurrentBucketActionCount()
		{
			if (_bucketRecords.IsValidIndex(_currentBucketID))
			{
				return _bucketRecords[_currentBucketID].actionCount;
			}
			return 0;
		}

		// 
		internal ref readonly ActionRecord GetCurrentAction() => ref _actionRecords[_currentActionID];
	
		// 
		public int GetBucketCount() => _bucketRecords.Length;
	
		// 
		public int GetCurrentBucketID() => _currentBucketID;

		// 
		public float GetBucketScoringProgress()
		{
			return Mathf.Clamp01((Time.time - _lastBucketScoringTime) / _bucketScoringInterval);
		}

		// 
		public float GetActionScoringProgress()
		{
			ref readonly BucketRecord bucket = ref _bucketRecords[_currentBucketID];
			return Mathf.Clamp01((Time.time - _lastActionScoringTime) / bucket.scoringInterval);
		}

		public int GetCurrentActionBucketID()
		{
			if (_actionRecords.IsValidIndex(_currentActionID))
			{
				return _actionRecords[_currentActionID].bucketID;
			}
			return -1;
		}

		public void StartLogic()
		{
			if (_running)
			{
				return;
			}
			
			_cachedManager = UtilityManager.GetInstance();
			
			_cachedManager.RegisterBrain(this);

			_running = true;

			InitExecutionContext();
		}

		public void StopLogic()
		{
			if (!_running)
			{
				return;
			}

			_running = false;

			if (_cachedManager)
			{
				_cachedManager.UnregisterBrain(this);
				
				UtilityManager.StopRoutine(_actionScoringRoutine);
				UtilityManager.StopRoutine(_bucketScoringRoutine);
				// UtilityManager.GetInstance().onGUI -= OnGUI;
			}
			_actionScoringRoutine = null;
		}
		
		public bool IsValidActionID(int actionID)
		{
			return _actionRecords.IsValidIndex(actionID);
		}
		
		public bool IsValidBucketID(int bucketID)
		{
			return _bucketRecords.IsValidIndex(bucketID);
		}

		internal void ForEachActionInBucket(int bucketID, ActionRefRO<ActionRecord> fn)
		{
			if (!_bucketRecords.IsValidIndex(bucketID))
			{
				return;
			}

			ref readonly BucketRecord bucket = ref _bucketRecords[bucketID];

			for (int i = 0; i < bucket.actionCount; i++)
			{
				int ActionID = _actionIndicesByScore[bucket.actionIndex + i];
				ref readonly ActionRecord aRecord = ref _actionRecords[ActionID];
				fn.Invoke(aRecord);
			}
		}

		internal void ForEachBucket(ActionRefRO<BucketRecord> fn)
		{
			for (int i = 0; i < _bucketIndicesByScore.Length; i++)
			{
				int bucketID = _bucketIndicesByScore[i];
				ref readonly BucketRecord record = ref _bucketRecords[bucketID];
				fn.Invoke(record);
			}
		}

		private EUtilitySelectionMethod _selectionMethod = EUtilitySelectionMethod.TopScore;
		private Coroutine _actionScoringRoutine = default;
		private Coroutine _bucketScoringRoutine = default;
		private UtilityContext _context = default;
		internal ActionRecord[] _actionRecords = Array.Empty<ActionRecord>();
		private BucketRecord[] _bucketRecords =  Array.Empty<BucketRecord>();
		private BucketExecutionConfig[] _buckets = Array.Empty<BucketExecutionConfig>();
		private UtilityManager _cachedManager = null;
		private bool _deactivatingAction = false;
		private bool _running = false;
		private int[] _actionIndicesByScore = Array.Empty<int>();
		private int[] _bucketIndicesByScore = Array.Empty<int>();
		private float _lastBucketScoringTime = 0;
		private float _lastActionScoringTime = 0;
		private int _currentBucketID = -1;
		private int _currentActionID = -1;
		private float _bucketScoringInterval = 5f;

		private UtilityBrain(){}

		internal struct ActionRecord
		{
			public int actionID;
			public int bucketID;
			public float score;
			public float cooldownEnd;
			public IUtilityAction template;
			public IUtilityAction instance;
			public Coroutine activationRoutine;
			public bool cancelled;
			public bool deactivating;
			public bool cancellable;

			public bool OnCooldown()
			{
				if (instance != null)
				{
					return false;
				}
				return cooldownEnd > Time.time;
			}
			public EUtilityActionStatus Status => instance != null ? instance.Status : EUtilityActionStatus.Inactive;
		}

		internal struct BucketRecord
		{
			public int ID;
			public float score;
			public string name;
			public int actionIndex;
			public int actionCount;
			public float scoringInterval;
			public float weight;
			public EUtilitySelectionMethod selectionMethod;
			public UtilityBucketSO bucketSO;
			public UtilityConsiderationSetSO considerations;
		}

		private void InitExecutionContext()
		{
			List<BucketRecord> buckets = new();
			List<ActionRecord> actions = new();
			List<int> bucketIndices = new();
			List<int> actionIndices = new();

			foreach(var bucketConfig in _buckets)
			{
				var bucketSO = bucketConfig.bucket;

				BucketRecord bucketRecord = new BucketRecord();
				bucketRecord.ID = buckets.Count;
				bucketRecord.name = bucketSO.BucketName;
				bucketRecord.actionIndex = actions.Count;
				bucketRecord.scoringInterval = bucketSO._actionScoringInterval;
				bucketRecord.bucketSO = bucketSO;
				bucketRecord.selectionMethod = bucketConfig.selectionMethod;
				bucketRecord.considerations = bucketConfig.considerations;
				bucketRecord.weight = bucketConfig.weight;
				
				int aCount = 0;

				foreach (var action in bucketSO.GetActions())
				{
					if (!action.Enabled)
					{
						continue;
					}
					aCount++;
					var record = new ActionRecord();
					record.actionID = actions.Count;
					record.template = action.InstantiateAction(default, default);
					record.bucketID = bucketRecord.ID;
					actions.Add(record);
					actionIndices.Add(actionIndices.Count);
				}
				bucketRecord.actionCount = aCount;
				buckets.Add(bucketRecord);
				bucketIndices.Add(bucketIndices.Count);
			}

			_currentBucketID = buckets.Count > 0 ? 0 : -1;
			_actionRecords = actions.ToArray();
			_bucketRecords = buckets.ToArray();
			_actionIndicesByScore = actionIndices.ToArray();
			_bucketIndicesByScore = bucketIndices.ToArray();

			StartRoutine(ref _actionScoringRoutine, ActionScoringRoutine);
			StartRoutine(ref _bucketScoringRoutine, BucketScoringRoutine);

			if (!_cachedManager)
			{
				_cachedManager = UtilityManager.GetInstance();
			}
			// _cachedManager.onGUI -= OnGUI;
			// _cachedManager.onGUI += OnGUI;
		}
		
		// 
		private void ScoreBuckets()
		{
			_lastBucketScoringTime = Time.time;
			for (int i = 0; i < _bucketRecords.Length; i++)
			{
				int bucketID = i;
				ref BucketRecord record = ref _bucketRecords[bucketID];
				record.score = GetBucketScore(record);
			}

			UtilityHelpers.SortIndicesByWeight(ref _bucketIndicesByScore, 0, _bucketIndicesByScore.Length, i =>
			{
				return _bucketRecords[i].score;
			}, false);

		}
		

		// 
		private void ScoreActions()
		{
			_lastActionScoringTime = Time.time;

			ref readonly BucketRecord bucket = ref _bucketRecords[_currentBucketID];

			for (int i = 0; i < bucket.actionCount; i++)
			{
				var actionID = _actionIndicesByScore[bucket.actionIndex + i];
				ref ActionRecord record = ref _actionRecords[actionID];
				record.score = record.template.GetTotalScore();
				record.cancellable = record.instance != null ? record.instance.CanCancelAction() : false;
			}

			UtilityHelpers.SortIndicesByWeight(ref _actionIndicesByScore, bucket.actionIndex, bucket.actionCount, i =>
			{
				return _actionRecords[i].score;
			}, false);
		}

		private void SetNextBucket()
		{
			_currentBucketID = _bucketIndicesByScore.First();
		}

		private void ResetAction()
		{
			_currentActionID = -1;
			onActionChanged?.Invoke(null);

			SetNextAction();
		}

		private void SetNextAction()
		{
			if (_deactivatingAction)
			{
				return;
			}

			// action is active and uncancellable
			if (_actionRecords.IsValidIndex(_currentActionID) && !_actionRecords[_currentActionID].cancellable)
			{
				return;
			}

			ref readonly BucketRecord currBucket = ref _bucketRecords[_currentBucketID];

			var nextIndex = SelectAction(currBucket.selectionMethod);

			// already running best action
			if (nextIndex >= 0 && nextIndex == _currentActionID)
			{
				return;
			}

			if (_actionRecords.IsValidIndex(_currentActionID))
			{
				ref readonly ActionRecord action = ref _actionRecords[_currentActionID];

				if (!action.deactivating)
				{
					CancelAction(action.actionID, ResetAction);
				}
			}
			else if(_actionRecords.IsValidIndex(nextIndex))
			{
				ActivateAction(nextIndex);
			}
		}

		// 
		private int SelectAction(EUtilitySelectionMethod method)
		{
			if (method == EUtilitySelectionMethod.RandomWeighted)
			{
				return GetRandomActionWeighted();
			}

			if (method == EUtilitySelectionMethod.TopScoreInterval)
			{
				return GetBestActionInterval();
			}

			return GetBestAction();
		}
		
		private int GetBestAction()
		{
			int ID = _currentActionID;

			ref readonly BucketRecord bucket = ref _bucketRecords[_currentBucketID];

			for (int i = 0; i < bucket.actionCount; i++)
			{
				int actionID = _actionIndicesByScore[bucket.actionIndex + i];
				ref readonly ActionRecord action = ref _actionRecords[actionID];
				if (action.OnCooldown())
				{
					continue;
				}

				if (Mathf.Approximately(action.score, 0f))
				{
					continue;
				}
				//
				return action.actionID;
			}
			return ID;
		}

		private int GetBestActionInterval()
		{
			return GetBestAction();
		}

		private int GetRandomActionWeighted()
		{
			ref readonly BucketRecord bucket = ref _bucketRecords[_currentBucketID];

			List<int> indices = new();

			for (int i = 0; i < bucket.actionCount; i++)
			{
				int actionID = _actionIndicesByScore[bucket.actionIndex + i];
				ref readonly ActionRecord action = ref _actionRecords[actionID];
				if (action.OnCooldown())
				{
					continue;
				}

				if (Mathf.Approximately(action.score, 0f))
				{
					continue;
				}
				//
				indices.Add(actionID);
			}
			
			var randIndex = UtilityMath.GetRandomArrayIndexWeighted(indices.ToArray(), GetActionScore);;

			return randIndex >= 0 ? randIndex : _currentActionID;
		}

		private float GetActionScore(in int actionID)
		{
			return _actionRecords[actionID].score;
		}

		private float GetBucketScore(in BucketRecord bucket)
		{
			var score = bucket.considerations ? bucket.considerations.GetScore(_context) : 0f;
			return score * bucket.weight;
		}

		void CancelAction(int actionID, Action onDone)
		{
			ref ActionRecord record = ref _actionRecords[actionID];
			record.cancelled = true;
			DeactivateAction(actionID, EUtilityActionStatus.Cancelled, onDone);
		}

		private void DeactivateAction(int actionID, EUtilityActionStatus status, Action onDone)
		{
			if (_deactivatingAction)
			{
				return;
			}

			_deactivatingAction = true;

			ref ActionRecord record = ref _actionRecords[actionID];
			
			UtilityManager.StopRoutine(record.activationRoutine);
			record.activationRoutine = null;

			if (record.instance != null)
			{
				record.instance.Status = status;
			}
			
			record.deactivating = true;
			UtilityManager.RunCoroutine(DeactivateActionRoutine(actionID), onDone);
		}

		private IEnumerator DeactivateActionRoutine(int actionID)
		{
			var instance = _actionRecords[actionID].instance;

			if (instance != null)
			{
				yield return instance.DeactivateAction();
			}
			
			ActionRecord action = _actionRecords[actionID];

			if (instance != null)
			{
				action.cooldownEnd = Time.time + instance.GetActionCooldown();
			}
			
			action.deactivating = false;
			_actionRecords[actionID] = action;
			DisposeActionInstance(actionID);
			yield return null;

			_deactivatingAction = false;
			
			yield return null;
		}

		private void ActivateAction(int actionID)
		{
			_currentActionID = actionID;

			ref ActionRecord record = ref _actionRecords[actionID];
			record.cancelled = false;
			
			record.instance = record.template.InstantiateAction(_context, new UtilityActionCallbacks
			{
				onActionFinished = OnActionFinished
			});

			record.instance.Status = EUtilityActionStatus.Active;

			record.activationRoutine = UtilityManager.RunCoroutine(record.instance.ActivateAction(), OnActionFinished);
			onActionChanged?.Invoke(record.template);
		}

		// called when action finishes early
		private void OnActionFinished()
		{
			DeactivateAction(_currentActionID, EUtilityActionStatus.Completed, ResetAction);
		}

		private void DisposeActionInstance(int actionID)
		{
			ref ActionRecord record = ref _actionRecords[actionID];

			if (record.instance != null)
			{
				if (record.instance.GetType().IsSubclassOf(typeof(UnityEngine.Object)))
				{
					UnityEngine.Object.Destroy(record.instance as UnityEngine.Object);
				}
			}
			record.instance = null;
		}

		private IEnumerator ActionScoringRoutine()
		{
			while (true)
			{
				BucketRecord bucket = _bucketRecords[_currentBucketID];
				float interval = bucket.scoringInterval;
				yield return new WaitUntil(NotDeactivatingAction);
				ScoreActions();
				SetNextAction();
				yield return _cachedManager.GetDelayInstance(interval);
			}
		}

		private IEnumerator BucketScoringRoutine()
		{
			while (true)
			{
				yield return new WaitUntil(NotDeactivatingAction);
				ScoreBuckets();
				SetNextBucket();
				yield return _cachedManager.GetDelayInstance(_bucketScoringInterval);
			}
		}

		private bool NotDeactivatingAction() => !_deactivatingAction;

		private IUtilityAction GetCurrentActionInstance()
		{
			return _actionRecords.IsValidIndex(_currentActionID)
			? _actionRecords[_currentActionID].instance
			: null;
		}

		private IUtilityAction GetCurrentActionTemplate()
		{
			return _actionRecords.IsValidIndex(_currentActionID)
			? _actionRecords[_currentActionID].template
			: null;
		}

		private static void StartRoutine(ref Coroutine outRef, Func<IEnumerator> fn)
		{
			outRef = UtilityManager.RunCoroutine(fn());
		}

	}
}