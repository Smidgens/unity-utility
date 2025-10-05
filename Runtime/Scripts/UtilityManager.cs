// smidgens @ github

// ReSharper disable All

using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0414

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using Routine = System.Collections.IEnumerator;

	// spawned by utility brain to run things like coroutines
	[AddComponentMenu("")]
	[DefaultExecutionOrder(-2)]
	internal sealed class UtilityManager : MonoBehaviour
	{
		public static UtilityManager GetInstance()
		{
			if (_instance == null && !_closing)
			{
				_instance = (new GameObject(nameof(UtilityManager))).AddComponent<UtilityManager>();
				_instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
			return _instance;
		}

		public event Action onUpdate = null;
		public event Action onGUI = null;

		internal void RegisterBrain(UtilityBrain Brain)
		{
			if (FindBrainIndex(Brain) > -1)
			{
				return;
			}

			TrackedBrain tbrain = new TrackedBrain();
			tbrain.brain = Brain;

			var blist = _brains.ToList();
			blist.Add(tbrain);

			_brains = blist.ToArray();
		}
		
		internal void UnregisterBrain(UtilityBrain Brain)
		{
			int index = FindBrainIndex(Brain);
			if (index < 0)
			{
				return;
			}
			var blist = _brains.ToList();
			blist.RemoveAt(index);
			_brains = blist.ToArray();
		}

		internal void ForEachTrackedBrain(ActionRefRO<TrackedBrain> fn)
		{
			for (int i = 0; i < _brains.Length; i++)
			{
				ref readonly TrackedBrain tbrain = ref _brains[i];
				fn.Invoke(tbrain);
			}
		}

		internal struct TrackedBrain
		{
			public UtilityBrain brain;
		}

		private static bool _closing = false;

		private TrackedBrain[] _brains = Array.Empty<TrackedBrain>();
		
		private Dictionary<float, WaitForSeconds> _cachedDelays = new Dictionary<float, WaitForSeconds>();
		
		public WaitForSeconds GetDelayInstance(float delay)
		{
			if (!_cachedDelays.TryGetValue(delay, out WaitForSeconds value))
			{
				_cachedDelays[delay] = new WaitForSeconds(delay);
			}
			return value;
		}
		
		private int FindBrainIndex(UtilityBrain brain)
		{
			for (int i = 0; i < _brains.Length; i++)
			{
				ref readonly TrackedBrain tbrain = ref _brains[i];
				if (tbrain.brain == brain)
				{
					return i;
				}
			}
			return -1;
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		public static void StopRoutine(Coroutine routine)
		{
			if (_instance && routine != null)
			{
				_instance.StopCoroutine(routine);
			}
		}

		public static Coroutine RunCoroutine(Routine routine, Action onDone = null)
		{
			return GetInstance().StartCoroutine(StartRoutine(routine, onDone));
		}

		private static UtilityManager _instance = null;

		private void Update()
		{
			onUpdate?.Invoke();
		}

		private void OnGUI()
		{
			onGUI?.Invoke();
			UtilityDebugHUD.DrawDebugGUI();
		}

		private static Routine StartRoutine(Routine routine, Action action)
		{
			yield return routine;
			action?.Invoke();
		}
	}
}