// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	public enum EUtilitySelectionMethod
	{
		// Always pick highest score
		TopScore,
		// Pick random within % of top score
		TopScoreInterval,
		// Treat scores as weights and pick random
		RandomWeighted,
	}
}

namespace Smidgenomics.Unity.UtilityAI
{
	public enum EUtilityActionStatus
	{
		Inactive,
		Active,
		Completed,
		Cancelled
	}
}