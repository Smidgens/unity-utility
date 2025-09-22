// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using ActionResult = System.Collections.IEnumerator;

	/**
	 * TODO:
	 * - activate
	 * - tick
	 */
	public interface IUtilityAction
	{
		public float GetScore(in UtilityContext context);

		public ActionResult OnActivate(UtilityContext context);

		internal event Action OnActionFinished;

		// Used to clone templates for execution
		internal IUtilityAction InstantiateAction();

	}

}