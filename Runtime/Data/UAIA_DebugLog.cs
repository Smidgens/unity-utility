// smidgens @ github

#pragma warning disable 0414

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using System.Collections;

	//[CreateAssetMenu]
	[UtilityAssetMD(displayName = "Debug Log")]
	internal sealed class UAIA_DebugLog : UtilityActionSO
	{
		public override IEnumerator OnActivate(UtilityContext Context)
		{
			
			yield return null;
		}

		[SerializeField] private string _debugText = "";
	}
}