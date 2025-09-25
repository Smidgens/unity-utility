// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using IEnumerator = System.Collections.IEnumerator;

	/**
	 * TODO:
	 * - activate
	 * - tick
	 */
	public interface IUtilityAction
	{
		// display info
		public string Name { get; }

		// selectable?
		public bool Enabled { get; }

		// currently cancelable?
		public bool CanCancelAction();

		// cooldown based on current state
		public float GetCooldown();

		// >= 0, not necessarily normalized
		public float GetTotalScore();

		// execution status
		public UtilityActionStatus GetStatus();

		// main logic execution logic
		public IEnumerator ActivateAction();

		// begin cancellation
		public IEnumerator CancelAction();

		internal UtilityActionStatus Status { get; set;  }

		// Used to clone templates for execution
		internal IUtilityAction InstantiateAction(UtilityContext context, UtilityActionCallbacks callbacks);

	}
}