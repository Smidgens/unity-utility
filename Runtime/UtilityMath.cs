// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	using System.Collections.Generic;
	using UnityEngine;

	internal static class UtilityMath
	{
		public static float ScoreConsiderations
		(
			UtilityContext context,
			IEnumerable<IUtilityConsideration> considerations,
			out int count
		)
		{
			// we can't use array length if items may be null
			count = 0;

			// consideration score
			float combinedScore = 1;

			foreach(var consideration in considerations)
			{
				// we really throttled someone's terrier if we allowed a null item in here
				if(consideration == null || !consideration.Enabled)
				{
					continue;
				}
				count++;
		
				float score = consideration.GetScore(context);

				// we don't need to continue, although we might want to know other consideration scores
				// for debugging purposes even if they wouldn't change the total result
				if(Mathf.Approximately(score, 0f))
				{
					return 0;
				}

				combinedScore *= score;
			}

			// see library for explanation of what's going on here
			return RenormalizeAggregateScore(count, combinedScore);
		}

		/**
		 * Dave Mark's "magic" utility formula to renormalize aggregate score
		 * (Bring small value from multiplying scores together closer to 1)
		 */
		public static float RenormalizeAggregateScore(int ConsiderationCount, float AggregateScore)
		{
			if(ConsiderationCount <= 0){ return 0; }
			float ModFactor = 1 - (1 / ConsiderationCount);
			float MakeupValue = (1 - AggregateScore) * ModFactor;
			return AggregateScore + (MakeupValue * AggregateScore);
		}
	}
}