// smidgens @ github

// ReSharper disable All

#pragma warning disable 0414

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using Routine = System.Collections.IEnumerator;

	// spawned by utility brain to run things like coroutines
	[AddComponentMenu("")]
	internal sealed class UtilityScriptHost : MonoBehaviour
	{
		public static UtilityScriptHost GetInstance()
		{
			if (_instance == null)
			{
				_instance = (new GameObject(nameof(UtilityScriptHost))).AddComponent<UtilityScriptHost>();
				_instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
			return _instance;
		}

		public event Action onUpdate = null;
		public event Action onGUI = null;

		public static void StopRoutine(Coroutine routine)
		{
			GetInstance().StopCoroutine(routine);
		}
		
		public static Coroutine RunCoroutine(Routine routine, Action onDone = null)
		{
			return GetInstance().StartCoroutine(StartRoutine(routine, onDone));
		}

		private static UtilityScriptHost _instance = null;

		private void Update()
		{
			onUpdate?.Invoke();
		}

		private void OnGUI()
		{
			onGUI?.Invoke();
		}

		private static Routine StartRoutine(Routine routine, Action action)
		{
			yield return routine;
			action?.Invoke();
		}
	}
}