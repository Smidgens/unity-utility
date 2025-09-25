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
	
	public ref struct UtilityBrainInitConfig
	{
		public IEnumerable<IUtilityAction> actions;
		public UtilitySelectionMethod selectionMethod;
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
