// smidgens @ github

// ReSharper disable All

#pragma warning disable 0414
#pragma warning disable 0067

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
		public IUtilityAction CurrentTemplate => GetCurrentAction()?.template;

		public event Action<IUtilityAction> onActionChanged = null;

		public static UtilityBrain CreateBrain(in UtilityBrainInitConfig config)
		{
			var brain = new UtilityBrain();
			brain._actionTemplates = config.actions != null ? config.actions.ToArray() : Array.Empty<IUtilityAction>();
			brain._selectionMethod = config.selectionMethod;
			brain._scoringInterval = config.scoringInterval;
			brain._context = config.context;
			return brain;
		}

		public void StartLogic()
		{
			_actionRecords.Clear();

			foreach (var action in _actionTemplates)
			{
				if (!action.Enabled)
				{
					continue;
				}
				
				var record = new ActionRecord();
				record.template = action;
				_actionRecords.Add(record);
			}
			_scoringRoutine = UtilityScriptHost.RunCoroutine(ScoringRoutine());
			UtilityScriptHost.GetInstance().onUpdate -= Tick;
			UtilityScriptHost.GetInstance().onUpdate += Tick;
			UtilityScriptHost.GetInstance().onGUI -= OnGUI;
			UtilityScriptHost.GetInstance().onGUI += OnGUI;
		}

		public void StopLogic()
		{
			if (_scoringRoutine == null)
			{
				return;
			}
			UtilityScriptHost.StopRoutine(_scoringRoutine);
			_scoringRoutine = null;
			UtilityScriptHost.GetInstance().onUpdate -= Tick;
			UtilityScriptHost.GetInstance().onGUI -= OnGUI;
		}

		private UtilitySelectionMethod _selectionMethod = UtilitySelectionMethod.Best;
		private IEnumerable<IUtilityAction> _actionTemplates = Array.Empty<IUtilityAction>();
		private float _scoringInterval = 1f;
		private UtilityContext _context = default;
		private Coroutine _scoringRoutine = default;
		private int _activeActionIndex = -1;
		private List<ActionRecord> _actionRecords = new();
		private bool _cancelingAction = false;

		private UtilityBrain(){}

		private sealed class ActionRecord
		{
			public float score;
			public float cooldownEnd;
			public IUtilityAction template;
			public IUtilityAction instance;
			public Coroutine routine;
			public bool interrupted;
			
			public bool OnCooldown() => cooldownEnd > Time.time;

			public bool IsCancelable()
			{
				return instance != null ? instance.CanCancelAction() : true;
			}
		}

		private void Tick()
		{
			// is this necessary?
		}
		
		private void OnGUI()
		{
			DrawDebugOverlay();
		}

		private void UpdateScores()
		{
			ActionRecord current = GetCurrentAction();
			float bestScore = 0;
			int bestIndex = -1;

			int i = -1;
			foreach (var record in _actionRecords)
			{
				i++;

				
				record.score = record.template.GetTotalScore();

				// 
				if (record.OnCooldown())
				{
					continue;
				}

				if (record.score > bestScore)
				{
					bestScore = record.score;
					bestIndex = i;
				}
			}

			// best action is already running
			if (bestIndex >= 0 && _activeActionIndex == bestIndex)
			{
				return;
			}

			if (current == null)
			{
				SelectAction(bestIndex);
			}
			else if (current.IsCancelable() && !_cancelingAction)
			{
				int newIndex = bestIndex;
				_cancelingAction = true;
				UtilityScriptHost.RunCoroutine(CancelActionRoutine(current), () =>
				{
					_cancelingAction = false;
					SelectAction(newIndex);
				});
			}
		}

		private IEnumerator CancelActionRoutine(ActionRecord action)
		{
			UtilityScriptHost.StopRoutine(action.routine);
			yield return action.instance.CancelAction();
			action.interrupted = true;
			action.instance.Status = UtilityActionStatus.Canceled;
			action.cooldownEnd = Time.time + action.instance.GetCooldown();
			DisposeAction(action);

			yield return null;
		}

		private void SelectAction(int index)
		{
			_activeActionIndex = index;
			var record = _actionRecords[index];
			record.interrupted = false;
			record.instance = record.template.InstantiateAction(_context, new UtilityActionCallbacks
			{
				onActionFinished = OnActionCompleted
			});
			record.routine = UtilityScriptHost.RunCoroutine(record.instance.ActivateAction(), OnActionCompleted);
			onActionChanged?.Invoke(record.template);
		}

		private void OnActionCompleted()
		{
			var record = GetCurrentAction();
			record.cooldownEnd = Time.time + Mathf.Max(record.instance.GetCooldown(), UtilityConstants.MIN_COOLDOWN);
			UtilityScriptHost.StopRoutine(record.routine);
			record.instance.Status = UtilityActionStatus.Completed;
			DisposeAction(record);
			_activeActionIndex = -1;
			onActionChanged?.Invoke(null);
		}

		private void DisposeAction(ActionRecord record)
		{
			if (record.instance.GetType().IsSubclassOf(typeof(UnityEngine.Object)))
			{
				UnityEngine.Object.Destroy(record.instance as UnityEngine.Object);
			}
			record.instance = null;
		}

		private IEnumerator ScoringRoutine()
		{
			while (true)
			{
				yield return new WaitUntil(() => !_cancelingAction);
				UpdateScores();
				yield return new WaitForSeconds(_scoringInterval);
			}
		}

		private ActionRecord GetCurrentAction()
		{
			if (_activeActionIndex < 0)
			{
				return null;
			}
			return _actionRecords[_activeActionIndex];
		}

		private void DrawDebugOverlay()
		{
			var srect = new Rect(0, 0, Screen.width, Screen.height);
			var style = GUI.skin.label;
			var height = style.CalcSize(new GUIContent("a")).y;

			var pstart = new Vector2(10, 10);
			var padding = 2f;

			var rheight = padding * 2f + height;

			var rwidth = 300f;

			var i = 0;
			foreach (var action in _actionRecords)
			{
				var yo = rheight * i;
				var pos = pstart + new Vector2(0, yo);
				
				var rect = new Rect(pos, new  Vector2(rwidth, rheight));

				var lrect = rect;
				lrect.height = height;
				lrect.center = rect.center;

				var textColor = Color.white;
				var bColor = Color.black;

				var onCooldown = action.OnCooldown();
				
				if (CurrentTemplate == action.template)
				{
					bColor = Color.green;
					textColor = Color.black;
				}
				else if (onCooldown)
				{
					bColor = action.interrupted ? Color.yellow : Color.blue;
					textColor = action.interrupted ? Color.black : Color.white;
				}
				
				
				UnityEditor.EditorGUI.DrawRect(rect, bColor);
				
				lrect.SliceLeft(5f);
				lrect.SliceRight(5f);

				var cdCol = lrect.SliceRight(50);

				var wCol = lrect.SliceRight(50);
				lrect.SliceRight(5f);

				var tcolor = GUI.color;
				GUI.color = textColor; 
				GUI.Label(lrect, action.template.Name, style);
				GUI.Label(wCol, action.score.ToString(), style);

				if (onCooldown)
				{
					var remainder = action.cooldownEnd - Time.time;
					int val = remainder < 1 ? (int)(remainder * 1000) : (int)remainder;
					var timeLabel = val + (remainder < 1 ? "ms" : "s");
					GUI.Label(cdCol, timeLabel, style);
				}
				GUI.color = tcolor;
				i++;
			}

		}


	}
}