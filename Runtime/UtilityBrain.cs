// smidgens @ github

// ReSharper disable All


#pragma warning disable 0414

namespace Smidgenomics.Unity.UtilityAI
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	
	/**
	 * TODO:
	 * - Tick brain
	 * - Current action
	 */
	internal sealed class UtilityBrain
	{
		public IUtilityAction CurrentAction => _currentAction.instance;
		
		public static UtilityBrain CreateBrain(in UtilityBrainInitConfig config)
		{
			var brain = new UtilityBrain();
			brain._actions = config.actions ?? Array.Empty<IUtilityAction>();
			brain._selectionMethod = config.SelectionMethod;
			return brain;
		}

		public void Tick()
		{
			if (!_active)
			{
				return;
			}
		}

		public void StartLogic()
		{
			if (_active)
			{
				return;
			}

			_active = true;
			
			_records.Clear();

			foreach (var action in _actions)
			{
				_records[action] = new ActionRecord();
			}
			
		}

		public void StopLogic()
		{
			if (!_active)
			{
				return;
			}
		}

		private UtilitySelectionMethod _selectionMethod = default;
		private IUtilityAction[] _actions = { };
		private float _bestScore = 0f;

		private ActiveAction _currentAction = default;

		// scoring interval
		private float _interval = 1f;
		private bool _active = false;
		private Dictionary<IUtilityAction, ActionRecord> _records = new();

		private List<ActionRecord> _actionRecords = new();

		private UtilityBrain(){}

		private struct ActiveAction
		{
			public int index;
			public IUtilityAction instance;
			public IUtilityAction template;
		}

		private class ActionRecord
		{
			public float lastUsed;
			public float lastScore;
			public float cooldown;
		}

		private void ActivateAction(UtilityContext context, IUtilityAction action)
		{
			UtilityScriptHost.GetInstance().RunCoroutine(action.OnActivate(context), OnActionFinished);
		}

		private void OnActionFinished()
		{
			// new action
			var record = GetRecord(_currentAction.template);
			record.cooldown = Time.time;
		}

		private ActionRecord GetRecord(IUtilityAction action)
		{
			return _records[action];
		}

		private void DelayAction()
		{
			
		}


	}
}