// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	public enum UtilitySelectionMethod
	{
		// Always 
		Best,
		// Pick random in interval around best score
		BestInterval
	}
}

namespace Smidgenomics.Unity.UtilityAI
{
	public enum UtilityActionStatus
	{
		Inactive,
		Active,
		Completed,
		Canceled
	}
}