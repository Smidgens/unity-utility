// smidgens @ github

// ReSharper disable All

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;

	/**
	 * TODO:
	 * - Consider if this can be templated or extended (custom context payload)
	 */
	public struct UtilityContext
	{
		public GameObject gameObject;
	}
}

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;

	[System.Serializable]
	public struct BucketExecutionConfig
	{
		public UtilityBucketSO bucket;
		public UtilityConsiderationSetSO considerations;
		public EUtilitySelectionMethod selectionMethod;

		[Min(0f)]
		public float weight;
	}
	
	public ref struct UtilityBrainInitConfig
	{
		public BucketExecutionConfig[] buckets;
		public EUtilitySelectionMethod selectionMethod;
		public float scoringInterval;
		public UtilityContext context;
	}
}

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	
	internal struct UtilityActionCallbacks
	{
		public Action onActionFinished;
	}
}

namespace Smidgenomics.Unity.UtilityAI
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;

	[System.Serializable]
	internal struct FloatInterval
	{
		public float min, max;
	}
}