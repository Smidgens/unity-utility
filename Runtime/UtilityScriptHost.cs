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
			}
			return _instance;
		}

		public void DelayAction(Action action)
		{
			
		}

		public Coroutine RunCoroutine(Routine routine, Action action)
		{
			return StartCoroutine(StartRoutine(routine, action));
		}

		private static UtilityScriptHost _instance = null;

		private Routine StartRoutine(Routine routine, Action action)
		{
			yield return routine;
			action();
		}
	}
}