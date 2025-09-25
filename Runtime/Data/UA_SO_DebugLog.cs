// smidgens @ github

#pragma warning disable 0414

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System.Collections;

	[DisplayName("Debug Log")]
	internal sealed class UA_SO_DebugLog : UtilityActionSO
	{
		public override IEnumerator ActivateAction()
		{
			yield return new WaitForSeconds(_duration);
			yield return null;
		}
		
		public override float GetCooldown()
		{
			return _cooldown;
		}

		// [Header("Debug Log")]
		[SerializeField] private string _debugText = "";
		
		[Min(0.1f)]
		[SerializeField] private float _duration = 1;
		
		[Min(UtilityConstants.MIN_COOLDOWN)]
		[SerializeField] internal float _cooldown = 1f;
	}
}