// smidgens @ github

namespace Smidgenomics.Unity.UtilityAI
{
	using System;

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class UtilityAssetMD : Attribute
	{
		public string displayName = null;
		public string category = null;
		
		public UtilityAssetMD(){}
	}
}